import promptSync from "prompt-sync";
import * as fs from "fs";

const prompt = promptSync();

const countriesData = JSON.parse(
  fs.readFileSync("./countryData.json", "utf-8")
);

interface CountryData {
  name: string;
  adjacentCountries: string[];
}

const getAdjacentCountryNameBy = (countryCode: string): CountryData | null => {
  const code = countryCode.toUpperCase();
  if (countriesData[code]) {
    return countriesData[code];
  } else {
    return null;
  }
};

const getAdjacentCountryNameByCountryCode = () => {
  while (true) {
    console.log("Write 'END' to exit.");
    const countryCode = prompt(
      "Enter a country code (e.g., IN, US, NZ): "
    ).trim();
    if (countryCode === "END") break;
    const countryData = getAdjacentCountryNameBy(countryCode);
    if (countryData) {
      console.log(
        `The country for code "${countryCode.toUpperCase()}" is: ${
          countryData.name
        } and adjacent countries are: ${countryData.adjacentCountries}`
      );
    } else {
      console.log(`Country code "${countryCode.toUpperCase()}" not found.`);
    }
    console.log("-----------------------------------------");
  }
};

getAdjacentCountryNameByCountryCode();
