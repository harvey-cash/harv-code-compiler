
using UnityEngine;

public class Command {
    public int line { private set; get; }
    public string script;

    public Command(int line, string script) {
        this.line = line;
        this.script = script;
    }

    public static Command[] SubCommands(Command root, string[] subscripts) {        
        return SubCommands(root.line, subscripts);
    }

    public static Command[] SubCommands(int line, string[] subscripts) {
        Command[] commands = new Command[subscripts.Length];
        for (int i = 0; i < subscripts.Length; i++) {
            commands[i] = SubCommand(line, subscripts[i]);
        }
        return commands;
    }

    public static Command SubCommand(int line, string subscript) {
        return new Command(line, subscript);
    }
}
