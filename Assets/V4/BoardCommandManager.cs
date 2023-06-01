using System;
using System.Collections.Generic;
using UnityEngine;

public class BoardCommandManager : MonoBehaviour
{
    private Board board;

    [SerializeReference] private List<GroupCommand> groupCommands;

    [SerializeReference, SubclassSelector] private List<Command> performedCommands;
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
        groupCommands = new List<GroupCommand>();
    }

    void AddCommand(Command command)
    {
        performedCommands.Add(command);
    }

    void DoCommand(int commandIndex)
    {
        if (commandIndex < 0 || commandIndex >= performedCommands.Count)
        {
            return;
        }
        performedCommands[commandIndex].DoCommand();
    }

    void UndoLastCommand()
    {
        if (performedCommands.Count == 0)
        {
            return;
        }
        performedCommands[^1].UndoCommand();
        performedCommands.Remove(performedCommands[^1]);
    }

    public void UndoLastGroupCommand()
    {
        if (performedCommands.Count == 0 || groupCommands.Count == 0)
        {
            return;
        }
        var groupCommand = groupCommands[^1];
        groupCommand.UndoCommandsInGroup();
        foreach (var command in groupCommand.GetCommandsInGroup())
        {
            performedCommands.Remove(command);
        }
        groupCommands.Remove(groupCommand);
    }

    public void AddAndDoCommand(Command command)
    {
        performedCommands.Add(command);
        command.DoCommand();
    }

    public void AddAndDoCommandToTheLastGroup(Command command)
    {
        groupCommands[^1].AddCommandToGroup(command);
        performedCommands.Add(command);
        command.DoCommand();
    }

    public void CreateNewCommandGroup(string name)
    {
        groupCommands.Add(new GroupCommand(name));
    }
}     