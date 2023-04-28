using System;
using System.Collections.Generic;
using UnityEngine;

class BoardCommandManager : MonoBehaviour
{
    private Board board;
    [SerializeField] private List<Command> performedCommands;
    public static BoardCommandManager instance;

    private void Awake()
    {
        instance = this;
        InitializeHistory();
    }

    void InitializeHistory()
    {
        board = GetComponent<Board>();
        performedCommands = new List<Command>();
    }

    public void AddCommand(Command command)
    {
        performedCommands.Add(command);
    }

    public void DoCommand(int commandIndex)
    {
        if (commandIndex < 0 || commandIndex >= performedCommands.Count)
        {
            return;
        }
        performedCommands[commandIndex].DoCommand();
    }

    public void UndoLastCommand()
    {
        if (performedCommands.Count == 0)
        {
            return;
        }
        performedCommands[^1].UndoCommand();
        performedCommands.Remove(performedCommands[^1]);
    }

    public void AddAndDoCommand(Command command)
    {
        performedCommands.Add(command);
        command.DoCommand();
    }
}