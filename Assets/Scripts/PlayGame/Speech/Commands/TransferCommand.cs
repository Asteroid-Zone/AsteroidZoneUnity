namespace PlayGame.Speech.Commands {
    
    /// <summary>
    /// This class represents a transfer command.
    /// </summary>
    public class TransferCommand : Command {

        public readonly int TransferAmount;

        /// <summary>
        /// Constructor for a transfer command.
        /// <remarks>Transfer commands are miner only.</remarks>
        /// </summary>
        /// <param name="transferAmount">Amount of resources to transfer.</param>
        public TransferCommand(int transferAmount) : base(CommandType.Transfer, false, true) {
            TransferAmount = transferAmount;
        }
        
    }
}