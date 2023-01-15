using UnityEngine;
using System.Text;
using System.Collections.Generic;

// FINITE STATE MACHINE
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
        State<A> prev = previus.Pop();
        changeState(prev);
    }
    public void update()
    {
        Debug.Log("CurentState: " + current.ToString());
        current.execute(agent);
    }
}


public abstract class Pathfinder {
	public Graph navGraph;
	public abstract List<int> findPath (int a, int b);
}

public abstract class PathAgent : MonoBehaviour {
    // 
    public StateMachine<PathAgent> fsm;
    public Renderer render;

    // Set from inspector
    public GameObject waypointSet;

	// Waypoints
	protected WaypointGraph waypoints;
	protected int? current;

	public List<int> path;
	protected Pathfinder pathfinder;

	public float speed;
	protected static float NEARBY = 0.2f;
	protected static System.Random rnd = new System.Random();

	public abstract Pathfinder createPathfinder ();

    public float wpDistance(int a, int b)
    {
        return Vector3.Distance(waypoints[a].transform.position, waypoints[b].transform.position);
    }

    void Start () {
		waypoints = new WaypointGraph (waypointSet);
		path = new List<int> ();
		pathfinder = createPathfinder ();
		pathfinder.navGraph = waypoints.navGraph;

        render = GetComponent<Renderer>();
        fsm = new StateMachine<PathAgent>(this);
        fsm.current = new InitialState();
        fsm.changeState(new InitialState());
        Interrupted = false;
    }

    public bool Interrupted { get; set; }

    void Update() { fsm.update(); }
	void FixedUpdate() {
		if (path.Count > 0 && !Interrupted) {
			GameObject next = waypoints[path[0]];
			Vector3 position = next.transform.position;
            transform.position = Vector3.MoveTowards (transform.position, position, speed);
		}
	}


    public bool moveToNearest()
    {
        if (path.Count == 0) return true;
        // Get the next waypoint position
        GameObject next = waypoints[path[0]];
        Vector3 there = next.transform.position;
        Vector3 here = transform.position;
        // Fix vertical waypoint position to prevent agent stuck by physics
        there.y = 0.3f;

        // Are we there yet?
        float distance = Vector3.Distance(here, there);
        if (distance < NEARBY)
        {
            // We're here
            current = path[0];
            path.RemoveAt(0);

            Debug.Log("Arrived at waypoint " + current);
        }
        return distance < NEARBY;
    }

    public bool Finished()
    {
        return path.Count == 0;
    }
    public void generatePathToNearest()
    {
        // Find the nearest waypoint
        int? target = waypoints.findNearest(transform.position);

        if (target != null)
        {
            // Go to target
            path.Clear();
            path.Add(target.Value);

            Debug.Log("Heading for nearest waypoint: " + target);
        }
        else
        {
            // Couldn't find a waypoint
            Debug.Log("Can't find nearby waypoint to target");
        }
    }

    public void planPath()
    {
        generateNewPath();
    }
    protected void generateNewPath() {

		if (current != null) {
			// We know where are
			List<int> nodes = waypoints.navGraph.nodes ();

			if (nodes.Count > 0) {
				// Pick a random node to aim for
				int target = nodes [rnd.Next (nodes.Count)];
				Debug.Log ("New target: " + target);
				// Find a path from here to there
				path = pathfinder.findPath (current.Value, target);
				Debug.Log ("New path: " + writePath(path));

			} else {
				// There are zero nodes
				Debug.Log ("No waypoints - can't select new target");
			}

		} else {
			// We don't know where we are

			// Find the nearest waypoint
			int? target = waypoints.findNearest (transform.position);

			if (target != null) {
				// Go to target
				path.Clear ();
				path.Add (target.Value);

				Debug.Log ("Heading for nearest waypoint: " + target);
			} else {
				// Couldn't find a waypoint
				Debug.Log ("Can't find nearby waypoint to target");
			}
		
		}
	}

	public static string writePath(List<int> path) {
		var s = new StringBuilder();
		bool first = true;
		foreach(int t in path) {
			if(first) {
				first = false;
			} else {
				s.Append(", ");
			}
			s.Append(t);
		}    
		return s.ToString();
	}

    /////// Actions ///////
    public void setBlackColor() { render.material.color = Color.black; }
    public void setWhiteColor() { render.material.color = Color.white; }
    public void setGrayColor() { render.material.color = Color.gray; }
    public void setRedColor() { render.material.color = Color.red; }
    public void setGreenColor() { render.material.color = Color.green; }
    public void setBlueColor() { render.material.color = Color.blue; }
    public void setYellowColor() { render.material.color = Color.yellow; }

}
/// <summary>
/// Initial state -> 
/// change color to black;
/// find nearest and move to the waypoint, 
/// when wp reached -> change state to ReachedWayPointState
/// 
/// ReachedWayPointState ->
/// change color to red;
/// then pause for 1 sec;
/// change color to green;
/// pause 1sec;
/// change color to blue;
/// pause 1sec;
/// plan path to the next node
/// change state to MoveToTargetWayPointState
/// 
/// MoveToTargetWayPointState ->
/// change color to "move color" (lets say "move color" its white color) 
/// when we reach any intermediate waypoint (between start wp and target wp) 
/// interrupt state to ReachedIntermediateState
/// when we reach finish wp -> change state to ReachedWayPointState
/// 
/// ReachedIntermediateState ->
/// change color to green
/// pause 1s
/// resume previous state
/// 
/// 
/// All states can be interrupted by 'space' from user (its simple pause everything)
/// PauseState ->
/// change color to gray
/// do nothing while user KeyUP 'space' button
/// 
///
/// </summary>


public class InitialState : State<PathAgent>
{
    public void enter(PathAgent agent)
    {
        // Action when entering initial state
        agent.setBlackColor();
        agent.generatePathToNearest();
    }

    public void execute(PathAgent agent)
    {
        if(agent.moveToNearest() == true)
        {
            // Change state to WPReached
            agent.fsm.changeState(new ReachedWayPointState());
        } 
        else if(Input.GetKeyDown(KeyCode.Space)) // Interruption
        {
            agent.fsm.Interrupt(new PauseState());
        }
    }

    public void exit(PathAgent agent) { }
}

public class ReachedWayPointState : State<PathAgent>
{
    double t = 0;
    static double DURATION = 1.5; // 1.5 sec before change color
    public void enter(PathAgent agent)
    {
        // Action when entering initial state
        agent.setRedColor();
    }

    public void execute(PathAgent agent)
    {
        if(Input.GetKeyDown(KeyCode.Space)) // Interruption
        {
            agent.fsm.Interrupt(new PauseState());
        }
        else if (t < DURATION)
        {
            // Add current duration to time counter
            t += Time.deltaTime;
        }
        else
        {
            t = 0;
            agent.fsm.changeState(new YellowState());
        }
    }

    public void exit(PathAgent agent) { }
}
public class YellowState : State<PathAgent>
{
    double t = 0;
    static double DURATION = 1.5; // 1.5 sec before change color
    public void enter(PathAgent agent) { agent.setYellowColor(); }
    public void execute(PathAgent agent)
    {
        if (Input.GetKeyDown(KeyCode.Space)) // Interruption
        {
            agent.fsm.Interrupt(new PauseState());
        }
        else if (t < DURATION)
        {
            // Add current duration to time counter
            t += Time.deltaTime;
        }
        else
        {
            t = 0;
            agent.fsm.changeState(new BlueState());
        }
    }

    public void exit(PathAgent agent) { }
}
public class BlueState : State<PathAgent>
{
    double t = 0;
    static double DURATION = 1.5; // 1.5 sec before change color
    public void enter(PathAgent agent) { agent.setBlueColor(); }
    public void execute(PathAgent agent)
    {
        if (Input.GetKeyDown(KeyCode.Space)) // Interruption
        {
            agent.fsm.Interrupt(new PauseState());
        }
        else if (t < DURATION)
        {
            // Add current duration to time counter
            t += Time.deltaTime;
        }
        else
        {
            t = 0;
            agent.planPath();
            agent.fsm.changeState(new MoveToTargetWayPointState());
        }
    }

    public void exit(PathAgent agent) { }
}

public class MoveToTargetWayPointState : State<PathAgent>
{
    public void enter(PathAgent agent) { agent.setWhiteColor(); }
    public void execute(PathAgent agent)
    {
        if (Input.GetKeyDown(KeyCode.Space)) // Interruption
        {
            agent.fsm.Interrupt(new PauseState());
        }
        else if (agent.Finished())
        {
            agent.fsm.changeState(new ReachedWayPointState());
        }
        else if(agent.moveToNearest() == true)
        {
            agent.fsm.Interrupt(new ReachedIntermediateState());
        }
    }

    public void exit(PathAgent agent) { }
}

public class ReachedIntermediateState : State<PathAgent>
{
    double t = 0;
    static double DURATION = 0.5; // 1.5 sec before change color
    public void enter(PathAgent agent) { agent.Interrupted = true; agent.setGreenColor(); }
    public void execute(PathAgent agent)
    {
        if (Input.GetKeyDown(KeyCode.Space)) // Interruption
        {
            agent.fsm.Interrupt(new PauseState());
        }
        else if (t < DURATION)
        {
            // Add current duration to time counter
            t += Time.deltaTime;
        }
        else
        {
            t = 0;
            agent.fsm.Resume();
        }
    }

    public void exit(PathAgent agent) {
        Debug.Log("-------------------INTER EXIT");
        agent.Interrupted = false;
    }
}

public class PauseState : State<PathAgent>
{
    public void enter(PathAgent agent) { agent.Interrupted = true; agent.setGrayColor(); }
    public void execute(PathAgent agent)
    {
        if (Input.GetKeyUp(KeyCode.Space)) // Interruption resume
        {
            agent.fsm.Resume();
        }
    }

    public void exit(PathAgent agent) { agent.Interrupted = false; }
}
