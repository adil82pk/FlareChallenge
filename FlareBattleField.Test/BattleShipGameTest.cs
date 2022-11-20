using Xunit;
using Microsoft.Extensions.Configuration;
using FlareBattleShip.Interfaces;
using FlareBattleShip.Classes;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using FlareBattleShip.Models;
namespace FlareBattleField.Test;

/// <summary>
/// Reference: https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices
/// </summary>
public class BattleShipGameTest
{
    [Theory]
    // inputBoardSize, battleShips, expectedBattleShips
    [InlineData(10, 0, 0)]
    [InlineData(20, 10, 10)]
    [InlineData(30, 15, 15)]
    public void InnitalizePosition_TestInputsProvided_ReturnsExpectedShipsCount(int inputBoardSize, int battleShips, int expectedBattleShips)
    {
        //Arrange
        var serviceProvider = InitializeDependencies(inputBoardSize, battleShips);
        var myBattleShipGame = serviceProvider.GetService<IBattleShipGame>();

        // Assert
        Assert.Equal(myBattleShipGame?.Ship.Count, expectedBattleShips);
    }

    [Theory]
    // Test my board contains the ships in the 2D array equals to the battleship passed as an input 
    // inputBoardSize, battleShips, expectedBattleShips
    [InlineData(10, 0, 0)]
    [InlineData(10, 1, 1)]
    [InlineData(10, 2, 2)]
    [InlineData(10, 3, 3)]
    [InlineData(10, 4, 4)]
    [InlineData(10, 5, 5)]
    public void MyShip_TestInputsProvided_ReturnsExpectedShipsCount(int inputBoardSize, int battleShips, int expectedBattleShips)
    {
        //Arrange
        var serviceProvider = InitializeDependencies(inputBoardSize, battleShips);
        var myBattleShipGame = serviceProvider.GetService<IBattleShipGame>();
        var opponentBattleShipGame = serviceProvider.GetService<IBattleShipGame>();
        var displayShips = true;

        var myBoard = MyBoard.GetMyBoard(myBattleShipGame.StrikeCoordinates, myBattleShipGame, opponentBattleShipGame, displayShips, inputBoardSize);

        // Assert
        Assert.Equal(expectedBattleShips, CountBattleShips(myBoard, "U"));
    }

    [Theory]
    // Test opponent board contains the ships in the 2D array equals to the battleship passed as an input 
    // inputBoardSize, battleShips, expectedBattleShips
    [InlineData(10, 0, 0)]
    [InlineData(10, 1, 1)]
    [InlineData(10, 2, 2)]
    [InlineData(10, 3, 3)]
    [InlineData(10, 4, 4)]
    [InlineData(10, 5, 5)]
    public void OpponetShip_TestInputsProvided_ReturnsExpectedShipsCount(int inputBoardSize, int battleShips, int expectedBattleShips)
    {
        //Arrange
        var serviceProvider = InitializeDependencies(inputBoardSize, battleShips);
        var myBattleShipGame = serviceProvider.GetService<IBattleShipGame>();
        var opponentBattleShipGame = serviceProvider.GetService<IBattleShipGame>();

        var opponentBoard = OpponentBoard.GetOpponentBoard(myBattleShipGame, opponentBattleShipGame, inputBoardSize);

        // Assert
        Assert.Equal(expectedBattleShips, CountBattleShips(opponentBoard, "U"));
    }

    /// <summary>
    /// Initialize dependencies
    /// </summary>
    /// <param name="inputBoardSize">Input board size</param>
    /// <param name="battleShips">Battle ships</param>
    /// <returns></returns>
    private ServiceProvider InitializeDependencies(int inputBoardSize, int battleShips)
    {
        // using in memory settings so that we do not have dependency on actual appsetting.json file.
        var inMemorySettings = new Dictionary<string, string> { { "AppConfig:BoardSize", inputBoardSize.ToString() } };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddTransient<IBattleShipGame>(x => new BattleShipGame(battleShips,
            int.Parse(configuration.GetSection("AppConfig:BoardSize").Value ?? "0".ToString())));
        return serviceCollection.BuildServiceProvider();
    }

    /// <summary>
    /// Count Battle ships.
    /// </summary>
    /// <param name="board">Board.</param>
    /// <param name="search">search.</param>
    /// <returns>Number of occurrences.</returns>
    private int CountBattleShips(string[,] board, string search)
    {
        int uBound0 = board.GetUpperBound(0);
        int uBound1 = board.GetUpperBound(1);
        int count = 0;
        for (int i = 0; i <= uBound0; i++)
        {
            for (int j = 0; j <= uBound1; j++)
            {
                string result = board[i, j];
                if (!string.IsNullOrEmpty(result) && result.Contains(search))
                {
                    count++;
                }
            }
        }

        return count;
    }
}