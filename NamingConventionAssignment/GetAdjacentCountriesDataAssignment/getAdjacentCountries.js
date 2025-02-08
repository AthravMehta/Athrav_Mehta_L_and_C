"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var prompt_sync_1 = require("prompt-sync");
var fs = require("fs");
var prompt = (0, prompt_sync_1.default)();
var countriesData = JSON.parse(fs.readFileSync("./countryData.json", "utf-8"));
var getAdjacentCountryNameBy = function (countryCode) {
    var code = countryCode.toUpperCase();
    if (countriesData[code]) {
        return countriesData[code];
    }
    else {
        return null;
    }
};
var getAdjacentCountryNameByCountryCode = function () {
    while (true) {
        console.log("Write 'END' to exit.");
        var countryCode = prompt("Enter a country code (e.g., IN, US, NZ): ").trim();
        if (countryCode === "END")
            break;
        var countryData = getAdjacentCountryNameBy(countryCode);
        if (countryData) {
            console.log("The country for code \"".concat(countryCode.toUpperCase(), "\" is: ").concat(countryData.name, " and adjacent countries are: ").concat(countryData.adjacentCountries));
        }
        else {
            console.log("Country code \"".concat(countryCode.toUpperCase(), "\" not found."));
        }
        console.log("-----------------------------------------");
    }
};
getAdjacentCountryNameByCountryCode();
