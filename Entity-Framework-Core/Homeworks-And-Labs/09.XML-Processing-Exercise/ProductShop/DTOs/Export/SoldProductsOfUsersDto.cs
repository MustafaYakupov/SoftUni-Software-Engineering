﻿using System.Xml.Serialization;

namespace ProductShop.DTOs.Export;

[XmlType("Product")]
public class SoldProductsOfUsersDto
{
    [XmlElement("name")]
    public string Name { get; set; }

    [XmlElement("price")]
    public decimal Price { get; set; }
}
