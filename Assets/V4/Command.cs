using System;
using UnityEngine.Events;

[Serializable]
public abstract  class Command
{
  public UnityEvent onDoCommand = new UnityEvent();

  public Command(UnityAction onDoCommand = null)
  {
    this.onDoCommand.AddListener(onDoCommand);
  }
  public virtual void DoCommand(bool force = false)
  {
    onDoCommand?.Invoke();
  }
  public abstract void UndoCommand(bool force=false);

}