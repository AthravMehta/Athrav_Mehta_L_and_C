import promptSync from "prompt-sync";
const input = promptSync();

const generateRandomNumberFrom = (diceFaceCount: number): number => {
  return Math.floor(Math.random() * diceFaceCount) + 1;
};

const playRollTheDiceGame = (): void => {
  const diceFaceCount: number = 6;
  let isPlaying: boolean = true;
  while (isPlaying) {
    const usersChoice: string = input("Ready to roll?? Enter Q to Quit: ");
    if (usersChoice?.toLowerCase() === "q") {
      isPlaying = false;
    } else {
      const diceRoll: number = generateRandomNumberFrom(diceFaceCount);
      console.log(`You have rolled a ${diceRoll}`);
    }
  }
};

playRollTheDiceGame();
