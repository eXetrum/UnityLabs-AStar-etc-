using UnityEngine;
using System.Collections;
using System;

public interface State<A>
{
    void enter(A agent);
    void execute(A agent);
    void exit(A agent);
}

public class StateMachine<A> 
{
    public A agent;
    public State<A> current;
    public System.Collections.Generic.Stack<State<A>> previus = new System.Collections.Generic.Stack<State<A>>();
    public StateMachine(A a) { agent = a; }

    public void changeState(State<A> next)
    {
        current.exit(agent);
        current = next;
        current.enter(agent);
    }

    public void Interrupt(State<A> interruptionState)
    {
        previus.Push(current);
        changeState(interruptionState);
    }
    public void Resume()
    {
        if (previus.Count == 0) return;
        current = previus.Pop();
        changeState(current);
    }
    public void update()
    {
        Debug.Log("CurentState: " + current.ToString());
        current.execute(agent);
    }
}

public class Agent : MonoBehaviour {

    public StateMachine<Agent> fsm;
    public Renderer render;
    /////// Actions ///////
    public void setBlackColor() { render.material.color = Color.black; }
    public void setWhiteColor() { render.material.color = Color.white; }
    public void setGrayColor() { render.material.color = Color.gray; }
    public void setRedColor() { render.material.color = Color.red; }
    public void setGreenColor() { render.material.color = Color.green; }
    public void setBlueColor() { render.material.color = Color.blue; }
    // Use this for initialization
    public void Start () {
        render = GetComponent<Renderer>();
        fsm = new StateMachine<Agent>(this);
        fsm.current = new InitialState();
        fsm.changeState(new InitialState());
    }
    // Update is called once per frame
    void Update() { fsm.update(); }
}

public class InitialState : State<Agent>
{
    public void enter(Agent agent)
    {
        // Action when entering initial state
        agent.setBlackColor();
    }

    public void execute(Agent agent)
    {
        // Change color state
        if (Input.GetKeyDown(KeyCode.R)) { agent.fsm.changeState(new RedState()); }
        else if (Input.GetKeyDown(KeyCode.G)) { agent.fsm.changeState(new GreenState()); }
        else if (Input.GetKeyDown(KeyCode.B)) { agent.fsm.changeState(new BlueState()); }
        // Movement states that can interrupt each other
        else if (Input.GetKeyDown(KeyCode.W)) { agent.fsm.Interrupt(new ForwardMoveState()); }
        else if (Input.GetKeyDown(KeyCode.S)) { agent.fsm.Interrupt(new BackMoveState()); }
        else if (Input.GetKeyDown(KeyCode.A)) { agent.fsm.Interrupt(new LeftMoveState()); }
        else if (Input.GetKeyDown(KeyCode.D)) { agent.fsm.Interrupt(new RightMoveState()); }
    }

    public void exit(Agent agent) { }
}

public class RedState : State<Agent>
{
    public void enter(Agent agent) { agent.setRedColor(); }
    public void execute(Agent agent)
    {
        // From first state we can back to the initial state with repeat 'R'
        // Change color state
        // If we in red state and receive condition 'R' again then back to the initial state
        if (Input.GetKeyDown(KeyCode.R)) { agent.fsm.changeState(new InitialState()); }
        else if (Input.GetKeyDown(KeyCode.G)) { agent.fsm.changeState(new GreenState()); }
        else if (Input.GetKeyDown(KeyCode.B)) { agent.fsm.changeState(new BlueState()); }
        // Movement states that can interrupt each other
        else if (Input.GetKeyDown(KeyCode.W)) { agent.fsm.Interrupt(new ForwardMoveState()); }
        else if (Input.GetKeyDown(KeyCode.S)) { agent.fsm.Interrupt(new BackMoveState()); }
        else if (Input.GetKeyDown(KeyCode.A)) { agent.fsm.Interrupt(new LeftMoveState()); }
        else if (Input.GetKeyDown(KeyCode.D)) { agent.fsm.Interrupt(new RightMoveState()); }
    }

    public void exit(Agent agent) { }
}

public class GreenState : State<Agent>
{
    public void enter(Agent agent) { agent.setGreenColor(); }
    public void execute(Agent agent)
    {
        // From first state we can back to the initial state with repeat 'G'
        // Change color state
        if (Input.GetKeyDown(KeyCode.R)) { agent.fsm.changeState(new RedState()); }
        // If we in green state and receive condition 'G' again then back to the initial state
        else if (Input.GetKeyDown(KeyCode.G)) { agent.fsm.changeState(new InitialState()); }        
        else if (Input.GetKeyDown(KeyCode.B)) { agent.fsm.changeState(new BlueState()); }
        // Movement states that can interrupt each other
        else if (Input.GetKeyDown(KeyCode.W)) { agent.fsm.Interrupt(new ForwardMoveState()); }
        else if (Input.GetKeyDown(KeyCode.S)) { agent.fsm.Interrupt(new BackMoveState()); }
        else if (Input.GetKeyDown(KeyCode.A)) { agent.fsm.Interrupt(new LeftMoveState()); }
        else if (Input.GetKeyDown(KeyCode.D)) { agent.fsm.Interrupt(new RightMoveState()); }
    }

    public void exit(Agent agent) { }
}
public class BlueState : State<Agent>
{
    public void enter(Agent agent) { agent.setBlueColor(); }
    public void execute(Agent agent)
    {
        // From first state we can back to the initial state with repeat 'B'
        // Change color state
        if (Input.GetKeyDown(KeyCode.R)) { agent.fsm.changeState(new RedState()); }
        else if (Input.GetKeyDown(KeyCode.G)) { agent.fsm.changeState(new GreenState()); }
        // If we in blue state and receive condition 'B' again then back to the initial state
        else if (Input.GetKeyDown(KeyCode.B)) { agent.fsm.changeState(new InitialState()); }
        // Movement states that can interrupt each other
        else if (Input.GetKeyDown(KeyCode.W)) { agent.fsm.Interrupt(new ForwardMoveState()); }
        else if (Input.GetKeyDown(KeyCode.S)) { agent.fsm.Interrupt(new BackMoveState()); }
        else if (Input.GetKeyDown(KeyCode.A)) { agent.fsm.Interrupt(new LeftMoveState()); }
        else if (Input.GetKeyDown(KeyCode.D)) { agent.fsm.Interrupt(new RightMoveState()); }
    }

    public void exit(Agent agent) { }
}

public class ForwardMoveState : State<Agent>
{
    static int movespeed = 1;
    Vector3 dir = Vector3.forward;

    public void enter(Agent agent) { agent.setWhiteColor(); }
    public void execute(Agent agent)
    {
        // End of forward movement -> resume previus state
        if (Input.GetKeyUp(KeyCode.W)) { agent.fsm.Resume(); }
        else if (Input.GetKeyDown(KeyCode.S)) { agent.fsm.Interrupt(new BackMoveState()); }
        else if (Input.GetKeyDown(KeyCode.A)) { agent.fsm.Interrupt(new LeftMoveState()); }
        else if (Input.GetKeyDown(KeyCode.D)) { agent.fsm.Interrupt(new RightMoveState()); }
        else
        {
            agent.transform.Translate(dir * movespeed * 0.01f);
        }
    }

    public void exit(Agent agent) { agent.setGrayColor(); }
}

public class BackMoveState : State<Agent>
{
    static int movespeed = 1;
    Vector3 dir = Vector3.back;
    public void enter(Agent agent) { agent.setWhiteColor(); }
    public void execute(Agent agent)
    {
        // End of back movement -> resume previus state
        if (Input.GetKeyUp(KeyCode.S)) { agent.fsm.Resume(); }
        else if (Input.GetKeyDown(KeyCode.W)) { agent.fsm.Interrupt(new ForwardMoveState()); }
        else if (Input.GetKeyDown(KeyCode.A)) { agent.fsm.Interrupt(new LeftMoveState()); }
        else if (Input.GetKeyDown(KeyCode.D)) { agent.fsm.Interrupt(new RightMoveState()); }
        else { agent.transform.Translate(dir * movespeed * 0.01f); }
    }
    public void exit(Agent agent) { agent.setGrayColor(); }
}

public class LeftMoveState : State<Agent>
{
    static int movespeed = 1;
    Vector3 dir = Vector3.left;
    public void enter(Agent agent) { agent.setWhiteColor(); }
    public void execute(Agent agent)
    {
        // End of left movement -> resume previus state
        if (Input.GetKeyUp(KeyCode.A)) { agent.fsm.Resume(); }
        else if (Input.GetKeyDown(KeyCode.W)) { agent.fsm.Interrupt(new ForwardMoveState()); }
        else if (Input.GetKeyDown(KeyCode.S)) { agent.fsm.Interrupt(new BackMoveState()); }
        else if (Input.GetKeyDown(KeyCode.D)) { agent.fsm.Interrupt(new RightMoveState()); }
        else { agent.transform.Translate(dir * movespeed * 0.01f); }
    }
    public void exit(Agent agent) { agent.setGrayColor(); }
}

public class RightMoveState : State<Agent>
{
    static int movespeed = 1;
    Vector3 dir = Vector3.right;
    public void enter(Agent agent) { agent.setWhiteColor(); }
    public void execute(Agent agent)
    {
        // End of right movement -> resume previus state
        if (Input.GetKeyUp(KeyCode.D)) { agent.fsm.Resume(); }
        else if (Input.GetKeyDown(KeyCode.W)) { agent.fsm.Interrupt(new ForwardMoveState()); }
        else if (Input.GetKeyDown(KeyCode.S)) { agent.fsm.Interrupt(new BackMoveState()); }
        else if (Input.GetKeyDown(KeyCode.A)) { agent.fsm.Interrupt(new LeftMoveState()); }
        else { agent.transform.Translate(dir * movespeed * 0.01f); }
    }
    public void exit(Agent agent) { agent.setGrayColor(); }
}