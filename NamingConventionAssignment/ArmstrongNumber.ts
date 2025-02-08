import promptSync from "prompt-sync";
const input = promptSync();

const calculateArmstrongSum = (number: number): number => {
  let armstrongSum = 0;
  let digitCount = 0;
  let remainingNumber = number;
  while (remainingNumber > 0) {
    digitCount++;
    remainingNumber = Math.floor(remainingNumber / 10);
  }
  remainingNumber = number;
  while (remainingNumber > 0) {
    const remainder = remainingNumber % 10;
    armstrongSum += Math.pow(remainder, digitCount);
    remainingNumber = Math.floor(remainingNumber / 10);
  }
  return armstrongSum;
};

const verifyArmstrongNumber = (): void => {
  const usersChoice = input("\nPlease enter a number to check for Armstrong: ");
  const number = parseInt(usersChoice, 10);
  if (isNaN(number)) {
    console.log("\nInvalid input. Please enter a valid number.");
    return;
  }
  const armstrongSum = calculateArmstrongSum(number);
  if (number === armstrongSum) {
    console.log(`\n${number} is an Armstrong number.\n`);
  } else {
    console.log(`\n${number} is not an Armstrong number.\n`);
  }
};

verifyArmstrongNumber();
