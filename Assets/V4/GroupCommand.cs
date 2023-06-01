using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GroupCommand
{
    [SerializeField] private string commandGroupName;
    [SerializeReference, SubclassSelector] private List<Command> commandsInGroup;
    public GroupCommand(string name)
    {
        commandGroupName = name;
        commandsInGroup = new List<Command>();
    }

    public List<Command> GetCommandsInGroup()
    {
        return commandsInGroup;
    }

    public void AddCommandToGroup(Command command)
    {
        commandsInGroup.Add(command);
    }

    public void RemoveCommandFromGroup(Command command)
    {
        commandsInGroup.Remove(command);
    }

    public void UndoCommandsInGroup(bool fast=false)
    {
        for (var index = commandsInGroup.Count - 1; index >= 0; index--)
        {
            var command = commandsInGroup[index];
            command.UndoCommand(fast);
        } 
    }
}