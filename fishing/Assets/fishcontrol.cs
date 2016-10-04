using UnityEngine;
using System.Collections;

public class fishcontrol : MonoBehaviour
{
    //http://answers.unity3d.com/questions/658492/how-do-i-add-fishing-to-my-game.html

    // Set public variables in Inspector
    public GameObject pole;                    // Assign a fishing pole prefab in Inspector; tilt & angle to your liking
    public Transform lineStart;                // Create/assign empty GameObject at the tip of the pole

    public int minFishingDistance;            // Min distance (7ish)
    public int maxFishingDistance;            // Max distance (28ish) 
    public bool wasInterrupted;                // Interrupted flag (moved, mob attacked, etc); public for outside access
    public float fishJumpingSpeed;            // Adjust jumpming speed (3ish)

    private RaycastHit hit;
    private GameObject scriptTarget;        // Create primitive sphere for scriptTarget, replace with fish if hit
    private GameObject fish;                // Create primitive "fish" (replace with model if you have one)
    private GameObject setHook;                // Sphere appears to click
    private float fishJumpHeight;            // Height of fish jump
    private Vector3 startPos;                // Hold player starting position on cast
    private bool fishjump, fishOn, noNibble;     // Simple states


    private void Awake()
    {
        wasInterrupted = false;


        if (maxFishingDistance <= 0)
            maxFishingDistance = 28;

        if (minFishingDistance < 0)
            minFishingDistance = 7;

        if (fishJumpingSpeed <= 0)
            fishJumpingSpeed = 3f;
    }


    void Update()
    {

        if (pole == null || lineStart == null)
            return;

        if (Input.GetButton("Fire1"))
        {
            ActionMain();
        }
    }

    private IEnumerator ActionMain()
    {

        // Scale according to your taste and WaitForSeconds value
        int fishingTimer = 1000;
        int thisRun = fishingTimer;
        bool runningMiniGame = false;

        // Randoms
        // How far out of water does fish breach? Shorter = more difficult
        fishJumpHeight = Random.Range(4, 7);
        Vector3 fishJumpTarget = new Vector3(hit.point.x, hit.point.y + fishJumpHeight, hit.point.z);

        // Random 0...1; noNibble means dead cast (but still cycles). You could adjust hard-coded ".2"
        // based on playerSkill, bait/gear quality, buff, etc
        float noBites = Random.value;
        if (noBites <= .10)
            noNibble = true;
        else
            noNibble = false;

        // Range (within fishingTimer) that fish will bite
        // Leave enough room for cycle to complete on low
        int biteAt = Random.Range(350, 500);

        // If LineRenderer components are somehow still around, get rid of them
        // to avoid errors, duplicate lines
        if (pole.gameObject.GetComponent<LineRenderer>() != null)
            DestroyImmediate(pole.gameObject.GetComponent<LineRenderer>());

        // Demo create LineRenderer via runtime script
        LineRenderer fishLine = pole.gameObject.AddComponent<LineRenderer>();
        Vector3 v1 = lineStart.position;
        Vector3 v2 = scriptTarget.transform.position;
        fishLine.SetColors(Color.green, Color.green);
        fishLine.SetWidth(0.005f, 0.007f);
        fishLine.SetVertexCount(2);
        fishLine.SetPosition(0, v1);
        fishLine.SetPosition(1, v2);
        fishLine.material = new Material(Shader.Find("Diffuse"));

        // Player movement cancels action
        Vector3 startPos = transform.position;

        // ------------- MAIN ----------------

        while (!wasInterrupted && thisRun > 0)
        {

            // Adjust LineRenderer component for animation sway, player rotate, fish jump, etc
            v1 = lineStart.position;
            fishLine.SetPosition(0, v1);
            v2 = scriptTarget.transform.position;
            fishLine.SetPosition(1, v2);


            if (!fishOn)
            {
                // Demo using Mathf.PingPong to achieve a little bounce to "bobber"
                float scrTgtY = Mathf.PingPong(Time.time, .1f) - .05f;
                scriptTarget.transform.position = new Vector3(scriptTarget.transform.position.x,
                                                              scriptTarget.transform.position.y + scrTgtY,
                                                              scriptTarget.transform.position.z);
            }

            // When thisRun equals Random biteAt, "Fish On!" (unless it's a dead cycle)
            if (!noNibble && thisRun == biteAt)
                FishOn();

            // Breach/jump fish; adjust "7" to your speed taste. Player has the interval between 
            // fish breach to apex of jump to react, otherwise fail

            if (fishjump)
            {
                scriptTarget.transform.Translate(Vector3.up * Time.deltaTime * fishJumpingSpeed);

                // Demo mini-game
                if (!runningMiniGame)
                {
                    StartCoroutine(MiniGame());
                    runningMiniGame = true;
                }

                // Has fish reached apex?
                if (scriptTarget.transform.position.y > fishJumpTarget.y)
                {
                    fishjump = false;
                    StopCoroutine("MiniGame()");
                    scriptTarget.AddComponent<Rigidbody>();
                    
                }
            }

            // If the fish falls back to water, cycle is over/fail; dual-purposing wasInterrupted
            // to end Coroutine
            if (fishOn && scriptTarget.transform.position.y < hit.point.y)
                wasInterrupted = true;

            // Movement cancels action
            if (startPos != transform.position)
            {
                Debug.Log("You stop fishing.");
                wasInterrupted = true;
            }

            // Decrement counter
            thisRun--;

            // If you change Wait len, remember to scale other values
            yield return new WaitForSeconds(0.01f);
        }

        // ------------- end WHILE ----------------

        if (noNibble)
        {

            GameObject fishingGUIText = new GameObject();

            GetComponent<GUIText>();
            GetComponent<GUIText>().transform.position = new Vector3(.43f, .85f);
            GetComponent<GUIText>().fontSize = 18;
            GetComponent<GUIText>().text = "The fish aren't biting";

            Destroy(fishingGUIText, 2.5f);
        }

        // Reset states for next run
        ActionMain();

        // The End.

    }


    private IEnumerator MiniGame()
    {

        GameObject fishingGUIText = new GameObject();
        GetComponent<GUIText>();
        GetComponent<GUIText>().transform.position = new Vector3(.35f, .8f);
        GetComponent<GUIText>().fontSize = 18;
        GetComponent<GUIText>().color = Color.green;
        GetComponent<GUIText>().text = "Set the hook! [Press G with Mouse in Circle]";
        Destroy(fishingGUIText, 2);

        setHook = GameObject.CreatePrimitive(PrimitiveType.Sphere) as GameObject;

        // Set "setHook" properties
        float setHookX = Random.Range(-3, 3);
        float setHookY = Random.Range(-1, 1);
        Vector3 setHookTarget = new Vector3(hit.point.x + setHookX, hit.point.y + fishJumpHeight + setHookY, hit.point.z);
        setHook.transform.position = setHookTarget;
        
        setHook.transform.localScale = new Vector3(.7f, .7f, .7f);
        setHook.name = "setHook";

        bool miniGameDone = false;

        while (fishjump && !miniGameDone)
        {

            // Press G to set hook
            if (Input.GetKeyUp(KeyCode.G))
            {

                Ray _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hitInfo = new RaycastHit();

                if (Physics.Raycast(_ray, out hitInfo, Mathf.Infinity))
                {
                    if (hitInfo.transform.name == setHook.name)
                    {
                        DestroyImmediate(setHook);
                        DestroyImmediate(fishingGUIText);
                        miniGameDone = ActionSuccess();
                    }
                }
            }

            yield return null;
        }

        DestroyImmediate(setHook);
        DestroyImmediate(fishingGUIText);
    }

    private void FishOn()
    {

        // This bit uses a Primitive Sphere. If you have a fish model you could
        // use that instead, swapping out this code for that

        // Get rid of "bobber"
        DestroyImmediate(scriptTarget.gameObject);

        // Create "fish"
        scriptTarget = GameObject.CreatePrimitive(PrimitiveType.Sphere) as GameObject;

        // Set fish properties
        scriptTarget.transform.position = hit.point;
       
        scriptTarget.transform.localScale = new Vector3(.7f, 1.2f, .2f);

        fishOn = true;
        fishjump = true;
    }


    private bool ActionSuccess()
    {
        // This is success code; there's too many things that you could be doing in your game
        // for me to guess at: add fish to inventory, bump fishing skill, determine how fast they clicked vs
        // how far away the hit.point was and give treasure map for great click, spawn mermaid/man, 
        // bump Fishing GUI score, etc.

        // To close, we'll clean-up and put success message up

        GameObject successGUIText = new GameObject();
        GetComponent<GUIText>();
        GetComponent<GUIText>().transform.position = new Vector3(.35f, .85f);
        GetComponent<GUIText>().fontSize = 18;
        GetComponent<GUIText>().color = Color.white;
        GetComponent<GUIText>().text = "You caught a nice fish";
        Destroy(successGUIText, 2.5f);

        return true;
    }

}

