import promptSync from "prompt-sync";
import * as fs from "fs";

const prompt = promptSync();

const countries = JSON.parse(fs.readFileSync("./countries.json", "utf-8"));

const getCountryNameBy = (countryCode: string): string | null => {
  const code = countryCode.toUpperCase();
  if (countries[code]) {
    return countries[code];
  } else {
    return null;
  }
};

const getCountryNameByCountryCode = () => {
  while (true) {
    console.log("Write 'END' to exit.");
    const countryCode = prompt(
      "Enter a country code (e.g., IN, US, NZ): "
    ).trim();
    if (countryCode === "END") break;
    const countryName = getCountryNameBy(countryCode);
    if (countryName) {
      console.log(
        `The country for code "${countryCode.toUpperCase()}" is: ${countryName}`
      );
    } else {
      console.log(`Country code "${countryCode.toUpperCase()}" not found.`);
    }
    console.log("-----------------------------------------");
  }
};

getCountryNameByCountryCode();
