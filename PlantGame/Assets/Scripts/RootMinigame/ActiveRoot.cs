using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ActiveRoot : MonoBehaviour {
    [HideInInspector] public float generatePointTimer = 0f;
    [HideInInspector] public float speed = 500f;

    [HideInInspector] public Vector2 target;
    public LayerMask obstacleMask;

    public SpriteRenderer[] tieredPointers;
    public AudioSource bendingSound;
    public AudioSource nutrientGrabSound;

    [HideInInspector] public SpriteRenderer currentPointer;
    public CircleCollider2D circleCollider;
    public Rigidbody2D rb;
    [HideInInspector] public LineRenderer stem;
    Quaternion lastRotation;
    [HideInInspector] public bool currentlyRotating;

    [HideInInspector] public float strain = 0f;
    [HideInInspector] public int colorIndex = 0;
    [HideInInspector] public int nutrientCount = 0;
    [HideInInspector] public RootMinigameManager manager;

    private void Awake() {
        stem = GetComponent<LineRenderer>();
        stem.SetPosition(0, new Vector3(transform.position.x, transform.position.y + 16, 0f));
        stem.SetPosition(1, transform.position);

        target = transform.position + (transform.right * speed * Time.deltaTime);
        currentPointer = tieredPointers[0];
        foreach (SpriteRenderer pointer in tieredPointers) {
            if (currentPointer != pointer) pointer.enabled = false;
            else pointer.enabled = true;
        }

        UpdateColor(Color.green);
    }

    // Update is called once per frame
    void Update() {
        stem.SetPosition(stem.positionCount - 1, transform.position);
    }

    private void FixedUpdate() {
        if (manager.paused) return;

        float angle = Mathf.Atan2(target.y - transform.position.y, target.x - transform.position.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));
        if (lastRotation != targetRotation) {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 400f * Time.deltaTime);
            lastRotation = transform.rotation;
            currentlyRotating = true;
        } else {
            currentlyRotating = false;
        }

        // Changing the plantedRoots list mid collision would cause errors, so this bool will be used to buffer a root end if its found
        bool rootCollided = false;

        // Detect collisions with other roots. 0 in EndRoot means the plant was unsuccessful
        foreach (PlantedRoot pr in RootMinigameManager.Instance.plantedRoots) {
            for (int i = 1; i < pr.stem.positionCount; i++) {
                if (Physics2D.Raycast(pr.stem.GetPosition(i), pr.stem.GetPosition(i - 1) - pr.stem.GetPosition(i), Vector2.Distance(pr.stem.GetPosition(i), pr.stem.GetPosition(i - 1)), (1 << 9))) {
                    Debug.Log("Collided with pre-existing root");
                    RootMinigameManager.Instance.soundEffects.PlayOneShot(RootMinigameManager.Instance.sound_collidewithroot);
                    rootCollided = true;
                }
            }
        }

        // Detect collisions with own root. 0 in EndRoot means the plant was unsuccessful
        for (int i = 1; i < stem.positionCount; i++) {
            if (Physics2D.Raycast(stem.GetPosition(i), stem.GetPosition(i - 1) - stem.GetPosition(i), Vector2.Distance(stem.GetPosition(i), stem.GetPosition(i - 1)), (1 << 9))) {
                Debug.Log("Collided with own root");
                RootMinigameManager.Instance.soundEffects.PlayOneShot(RootMinigameManager.Instance.sound_collidewithroot);
                rootCollided = true;
            }
        }

        if (rootCollided) EndRoot(0);

        rb.velocity = transform.right * speed * Time.deltaTime;

        // if (transform.rotation.eulerAngles.z < 190f || transform.rotation.eulerAngles.z > 350f) {
        if (transform.rotation.eulerAngles.z < 215f || transform.rotation.eulerAngles.z > 325f) {
            float testAngle = transform.rotation.eulerAngles.z;
            if (testAngle > 180f) testAngle -= 180f;
            float difference = Mathf.Abs(90f - testAngle);

            strain += (Time.deltaTime * 0.02f * (90f - difference));

            // Bending sound that gets higher and higher pitch the closer the root is to breaking from tension
            if (!bendingSound.isPlaying) bendingSound.Play();
            bendingSound.volume = 0.4f + ((strain / 4f) * 0.6f);
            bendingSound.pitch = 0.7f + (strain * 2f);

            // Fades to yellow when straining, more rapidly the higher strained it is
            UpdateColor(new Color(Mathf.Lerp(stem.endColor.r, 1f, strain / 7f), 1f, 0f));

            // Refreshes size
            RefreshStemStrengthVisual();
        } else UpdateColor(new Color(Mathf.Lerp(stem.endColor.r, 0f, Time.deltaTime * 0.5f), 1f, 0f));

        if (strain >= 4f) {
            Debug.Log("Strained");
            EndRoot(0);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Collision")) {
            Debug.Log("Collided with obstacle");
            RootMinigameManager.Instance.soundEffects.PlayOneShot(RootMinigameManager.Instance.sound_collidewithrock);
            EndRoot(0);
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Water")) {
            EndRoot(nutrientCount);
        }
    }

    void EndRoot(int nutrientCount) {
        // Snapping the root to where the collider landed looks more natural
        stem.SetPosition(stem.positionCount-1, circleCollider.transform.position);

        // Before destroying this root's controller, we create a copy static root
        GameObject plantedRoot = Instantiate(RootMinigameManager.Instance.plantedRootPrefab, transform.position, Quaternion.identity);
        PlantedRoot plantedRootDetail = plantedRoot.GetComponent<PlantedRoot>();
        plantedRootDetail.stem.positionCount = stem.positionCount;
        Vector3[] positionCopy = new Vector3[stem.positionCount];
        stem.GetPositions(positionCopy);
        plantedRootDetail.stem.SetPositions(positionCopy);
        plantedRootDetail.stem.startColor = stem.startColor;
        plantedRootDetail.stem.endColor = Color.green;
        plantedRootDetail.stem.endWidth = stem.endWidth;
        plantedRootDetail.colorIndex = colorIndex;
        plantedRootDetail.nutrientCount = nutrientCount;

        if (nutrientCount == 0) {
            RootMinigameManager.Instance.soundEffects.PlayOneShot(RootMinigameManager.Instance.sound_rootsnap);
            plantedRootDetail.stem.startWidth = stem.startWidth;
            plantedRootDetail.stem.startColor = RootMinigameManager.Instance.deadRootColor;
            plantedRootDetail.stem.endColor = RootMinigameManager.Instance.deadRootColor;
            RootMinigameManager.Instance.rootWasSuccessful = false;
        } else {
            RootMinigameManager.Instance.soundEffects.PlayOneShot(RootMinigameManager.Instance.sound_collidewithaquifer);
            RootMinigameManager.Instance.rootWasSuccessful = true;
        }

        RootMinigameManager.Instance.plantedRoots.Add(plantedRootDetail);
        RootMinigameManager.Instance.SaveRoots();

        Destroy(this.gameObject);
    }

    public void AddNutrient() {
        // Grabbing a nutrient cures strain
        strain = 0f;
        UpdateColor(Color.green);
        RefreshStemStrengthVisual();

        // Sound and visual effects
        if (nutrientCount < 3) {
            RootMinigameManager.Instance.nutrientGrabbedFlash = 0.75f;
            nutrientGrabSound.pitch = 0.75f + (0.5f * nutrientCount);
        } else {
            RootMinigameManager.Instance.nutrientGrabbedFlash = 0.35f;
            nutrientGrabSound.pitch = 0.5f;
        }

        // Play sound regardless of nutrient count
        nutrientGrabSound.Play();

        // Cancel incrementing if it wouldn't do anything
        if (nutrientCount >= 3) return;

        speed += 150f;
        nutrientCount++;
        currentPointer = tieredPointers[nutrientCount];
        foreach (SpriteRenderer pointer in tieredPointers) {
            if (currentPointer != pointer) pointer.enabled = false;
            else pointer.enabled = true;
        }

        stem.startColor = RootMinigameManager.Instance.rootStartColors[colorIndex, nutrientCount - 1];
    }

    public void RefreshStemStrengthVisual() {
        float newScaleFactor = 1f - (strain / 5f);
        transform.localScale = new Vector3(newScaleFactor, newScaleFactor, newScaleFactor);
        stem.endWidth = 0.25f - (strain / 22f);
    }

    void UpdateColor(Color c) {
        currentPointer.color = c;
        stem.endColor = c;
    }
}
