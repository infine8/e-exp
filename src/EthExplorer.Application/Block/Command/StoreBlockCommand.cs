using EthExplorer.Domain.Block.Entities;

namespace EthExplorer.Application.Block.Command;

public record StoreBlockCommand(BlockEntity Block) : ICommand;