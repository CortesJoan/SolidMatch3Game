public abstract  class Command
{
  public abstract void DoCommand(bool force=false);
  public abstract void UndoCommand(bool force=false);

}