using UnityEngine;

public class PlayerMovement : Character
{
    NPCManager manager;

    private void Start()
    {
        manager = FindObjectOfType<NPCManager>();
    }

    // Update is called once per frame
    void Update()
    {
        move.x = Input.GetAxisRaw("Horizontal");
        move.y = Input.GetAxisRaw("Vertical");

        bool openedMap = Input.GetKeyDown(KeyCode.M);

        UpdateAnimator();

        if (openedMap)
        {
            ObjectStore.instance.worldManager.worldImage.SetActive(!ObjectStore.instance.worldManager.worldImage.activeInHierarchy);
        }
    }

    private void FixedUpdate()
    {
        MoveTowardsTarget();

        if (manager != null && WorldInfo.spawnAnimals)
            manager.SpawnNewCreatures(transform.position);

        if (move.sqrMagnitude > 0)
            AudioManager.PlayFootstep();

        //rigidBody.velocity = move.normalized * moveSpeed;
    }
}
