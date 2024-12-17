"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var prompt_sync_1 = require("prompt-sync");
var fs = require("fs");
var prompt = (0, prompt_sync_1.default)();
var countries = JSON.parse(fs.readFileSync("./countries.json", "utf-8"));
var getCountryNameByCode = function (countryCode) {
    var code = countryCode.toUpperCase();
    if (countries[code]) {
        return countries[code];
    }
    else {
        return null;
    }
};
var main = function () {
    var userInput = prompt("Enter a country code (e.g., IN, US, NZ): ").trim();
    var countryName = getCountryNameByCode(userInput);
    if (countryName) {
        console.log("The country for code \"".concat(userInput.toUpperCase(), "\" is: ").concat(countryName));
    }
    else {
        console.log("Country code \"".concat(userInput.toUpperCase(), "\" not found."));
    }
};
main();
