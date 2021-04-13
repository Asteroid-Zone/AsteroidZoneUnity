namespace PlayGame.Speech.Commands {
    public class TransferCommand : Command {

        public readonly int TransferAmount;

        public TransferCommand(int transferAmount) : base(CommandType.Transfer, false, true) {
            TransferAmount = transferAmount;
        }
        
    }
}