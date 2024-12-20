﻿namespace Invoices.DataProcessor;

using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text;
using Invoices.Data;
using Invoices.Data.Models;
using Invoices.Data.Models.Enums;
using Invoices.DataProcessor.ImportDto;
using Invoices.Utilities;
using Newtonsoft.Json;

public class Deserializer
{
    private const string ErrorMessage = "Invalid data!";

    private const string SuccessfullyImportedClients
        = "Successfully imported client {0}.";

    private const string SuccessfullyImportedInvoices
        = "Successfully imported invoice with number {0}.";

    private const string SuccessfullyImportedProducts
        = "Successfully imported product - {0} with {1} clients.";

    // XML
    public static string ImportClients(InvoicesContext context, string xmlString)
    {
        StringBuilder sb = new StringBuilder();

        XmlHelper xmlHelper = new XmlHelper();
        const string xmlRoot = "Clients";

        // Valid models to import into the DB!
        ICollection<Client> clientsToImport = new HashSet<Client>();

        ImportClientDto[] deserializedClients = xmlHelper.Deserialize<ImportClientDto[]>(xmlString, xmlRoot);

        foreach (var clientDto in deserializedClients)
        {
            if (!IsValid(clientDto))
            {
                // The DTO has invalid data!
                sb.AppendLine(ErrorMessage);
                continue;
            }

            ICollection<Address> addressesToImport = new List<Address>();

            foreach (var addressDto in clientDto.Addresses)
            {
                if (!IsValid(addressDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                };

                Address newAddress = new Address()
                {
                    StreetName = addressDto.StreetName,
                    StreetNumber = addressDto.StreetNumber,
                    PostCode = addressDto.PostCode,
                    City = addressDto.City,
                    Country = addressDto.Country,
                };

                addressesToImport.Add(newAddress);
            }

            Client newClient = new Client()
            {
                Name = clientDto.Name,
                NumberVat = clientDto.NumberVat,
                Addresses = addressesToImport,
            };

            clientsToImport.Add(newClient);
            sb.AppendLine(String.Format(SuccessfullyImportedClients, clientDto.Name));
        }

        context.Clients.AddRange(clientsToImport); // EF will import both ne clients and new addresses
        context.SaveChanges();

        return sb.ToString().Trim();
    }

    // JSON
    public static string ImportInvoices(InvoicesContext context, string jsonString)
    {
        StringBuilder sb = new StringBuilder();

        ICollection<Invoice> invoicesToImport = new List<Invoice>();

        ImportInvoiceDto[] deserizlizedInvoices = JsonConvert.DeserializeObject<ImportInvoiceDto[]>(jsonString)!;

        foreach (var invoiceDto in deserizlizedInvoices)
        {
            if (!IsValid(invoiceDto))
            {
                sb.AppendLine(ErrorMessage);
                continue;
            }

            // Validate issueDate
            bool isIssueDateValid = DateTime
                .TryParse(invoiceDto.IssueDate, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime issueDate); // Parsed into issueDate
            
            // Validate dueDate
            bool isDueDateValid = DateTime
                .TryParse(invoiceDto.DueDate, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dueDate); // Parsed into dueDate

            // DateTime.Compare(t1, t2)
            // -> -1 => t1 is before t2
            // -> 0 => t1 is t2
            // -> 1 => t1 is after t2

            if (isIssueDateValid == false || isDueDateValid == false || DateTime.Compare(dueDate, issueDate) < 0)
            {
                sb.AppendLine(ErrorMessage);
                continue;
            }

            if (!context.Clients.Any(c => c.Id == invoiceDto.ClientId))
            {
                // Non-existing client

                sb.AppendLine(ErrorMessage);
                continue;
            }

            Invoice invoice = new Invoice()
            {
                Number = invoiceDto.Number,
                IssueDate = issueDate,  // From the tryParse
                DueDate = dueDate,
                Amount = invoiceDto.Amount,
                CurrencyType = (CurrencyType)invoiceDto.CurrencyType,
                ClientId = invoiceDto.ClientId,
            };

            invoicesToImport.Add(invoice);
            sb.AppendLine(String.Format(SuccessfullyImportedInvoices, invoiceDto.Number));
        }

        context.Invoices.AddRange(invoicesToImport);
        context.SaveChanges();

        return sb.ToString().Trim();
    }

    // Mapping table is being used
    public static string ImportProducts(InvoicesContext context, string jsonString)
    {
        StringBuilder sb = new StringBuilder();

        ICollection<Product> productsToImport = new List<Product>();

        ImportProductDto[] deserializedProducts = JsonConvert.DeserializeObject<ImportProductDto[]>(jsonString)!;

        foreach (var productDto in deserializedProducts)
        {
            if (!IsValid(productDto))
            {
                sb.AppendLine(ErrorMessage);
                continue;
            }

            Product newProduct = new Product()
            { 
                Name = productDto.Name,
                Price = productDto.Price,
                CategoryType = (CategoryType)productDto.CategoryType,
            };

            // Product client connection is many to many
            ICollection<ProductClient> productClientsToImport = new List<ProductClient>();

            foreach (var clientId in productDto.Clients.Distinct())
            {
                if (!context.Clients.Any(c => c.Id == clientId))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                ProductClient newProductClient = new ProductClient()
                {
                    ClientId = clientId,
                    Product = newProduct,
                };

                productClientsToImport.Add(newProductClient);
            }

            newProduct.ProductsClients = productClientsToImport;

            productsToImport.Add(newProduct);

            sb.AppendLine(String.Format(SuccessfullyImportedProducts, productDto.Name, productClientsToImport.Count));
        }

        context.Products.AddRange(productsToImport);
        context.SaveChanges();

        return sb.ToString().Trim();
    }

    public static bool IsValid(object dto)
    {
        var validationContext = new ValidationContext(dto);
        var validationResult = new List<ValidationResult>();

        return Validator.TryValidateObject(dto, validationContext, validationResult, true);
    }
} 
