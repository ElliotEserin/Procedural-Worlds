using UnityEngine;

public class PlayerMovement : Character
{
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

        //rigidBody.velocity = move.normalized * moveSpeed;
    }
}
