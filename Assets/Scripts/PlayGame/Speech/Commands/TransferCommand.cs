namespace PlayGame.Speech.Commands {
    public class TransferCommand : Command {

        public readonly int transferAmount;

        public TransferCommand(int transferAmount) : base(CommandType.Transfer, false, true) {
            this.transferAmount = transferAmount;
        }
        
    }
}