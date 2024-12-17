import promptSync from "prompt-sync";
const input = promptSync();

const isValidNumber = (inputValue: string): boolean => {
  const number = parseInt(inputValue, 10);
  return !isNaN(number) && number >= 1 && number <= 100;
};

const playGuessingGame = (): void => {
  const secretNumber = Math.floor(Math.random() * 100) + 1;
  let isCorrectGuess = false;
  let guessCount = 0;
  console.log("Guess a number between 1 and 100:");
  while (!isCorrectGuess) {
    const usersChoice = input("Enter your guess: ");
    if (!isValidNumber(usersChoice)) {
      console.log("Invalid input. Please enter a number between 1 and 100.");
      continue;
    }
    const currentGuess = parseInt(usersChoice, 10);
    guessCount++;
    if (currentGuess < secretNumber) {
      console.log("It's Lower. Guess higher number again.");
    } else if (currentGuess > secretNumber) {
      console.log("It's Higher. Guess lower number again.");
    } else {
      console.log(
        `Congratulations! You guessed the number in ${guessCount} attempts!`
      );
      isCorrectGuess = true;
    }
  }
};

playGuessingGame();
