using UnityEngine;

// Used strictly for debugging purposes.
public sealed class InteractableDebugger : MonoBehaviour
{
    [SerializeField] private GameObject testInteractable = null;
    [SerializeField] private PlayerController withPlayer = null;

    private IInteractable toTest;
    private bool lastPromptVisible = false;

    private void Start()
    {
        toTest = testInteractable.GetComponent<IInteractable>();
        if (toTest != null)
        {
            toTest.OnPromptEnter(withPlayer);
            toTest.InteractionComplete += () => Debug.Log("Interaction Completed");
        }
        else
            Debug.LogError("The test interactable must implement IInteractable!");
    }
    private void Update()
    {
        if (lastPromptVisible != toTest.PromptVisible)
        {
            lastPromptVisible = !lastPromptVisible;
            Debug.Log($"{Time.time}: Prompt became {(toTest.PromptVisible? "visible" : "hidden")}.");
        }
        if (Input.GetKeyDown(KeyCode.T))
            toTest.Interact();
    }
}
