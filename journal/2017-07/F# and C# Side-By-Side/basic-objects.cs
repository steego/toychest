public class Task {
    //  In F#, we don't need 3 lines for our constructor
    //  The parameters are moved to the line where
    //  we define our class
    public Task(string name, int priority, bool complete) {
        this.Name = name;
        this.Priority = priority;
        this.Complete = complete;
    }

    public string Name { get; private; }
    public int Priority { get; private; }
    public bool Complete { get; private; }
}