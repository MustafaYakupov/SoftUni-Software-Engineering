function solve(namesArray) {
    namesArray
    .sort((a, b) => a.localeCompare(b))
    .forEach((name, index) => {
        console.log(`${index + 1}.${name}`);
    });;
} 

solve(["John", "Bob", "Christina", "Ema"]);