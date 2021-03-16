﻿namespace PlayGame.Speech.Commands {
    public class TransferCommand : Command {

        public readonly int transferAmount;

        public TransferCommand(int transferAmount) : base(CommandType.Transfer, false) {
            this.transferAmount = transferAmount;
        }
        
    }
}