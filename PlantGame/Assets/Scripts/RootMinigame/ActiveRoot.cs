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

    [HideInInspector] public SpriteRenderer currentPointer;
    public CircleCollider2D circleCollider;
    public Rigidbody2D rb;
    [HideInInspector] public LineRenderer stem;
    Quaternion lastRotation;
    [HideInInspector] public bool currentlyRotating;

    [HideInInspector] public float strain = 0f;
    [HideInInspector] public int nutrientCount = 0;
    [HideInInspector] public RootMinigameManager manager;

    private void Awake() {
        stem = GetComponent<LineRenderer>();
        stem.SetPosition(0, new Vector3(transform.position.x, transform.position.y + 16));
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

        /*
        // Detect collisions with aquifers. true in EndRoot means the plant was successful
        foreach (GameObject a in RootMinigameManager.Instance.aquifers) {
            if (Vector2.Distance(transform.position, a.transform.position) <= ((a.transform.localScale.x / 2f) + circleCollider.radius)) {
                EndRoot(true);
            }
        }

        foreach (GameObject o in RootMinigameManager.Instance.obstacles) {
            if (Vector2.Distance(transform.position, o.transform.position) <= ((o.transform.localScale.x / 2f) + circleCollider.radius)) {
                EndRoot(false);
            }
        }
        */

        // Detect collisions with other roots. false in EndRoot means the plant was unsuccessful
        foreach (PlantedRoot pr in RootMinigameManager.Instance.plantedRoots) {
            for (int i = 1; i < pr.stem.positionCount; i++) {
                if (Physics2D.Raycast(pr.stem.GetPosition(i), pr.stem.GetPosition(i - 1) - pr.stem.GetPosition(i), Vector2.Distance(pr.stem.GetPosition(i), pr.stem.GetPosition(i - 1)))) {
                    EndRoot(false);
                }
            }
        }

        // Detect collisions with own root. false in EndRoot means the plant was unsuccessful
        for (int i = 1; i < stem.positionCount; i++) {
            if (Physics2D.Raycast(stem.GetPosition(i), stem.GetPosition(i - 1) - stem.GetPosition(i), Vector2.Distance(stem.GetPosition(i), stem.GetPosition(i - 1)))) {
                EndRoot(false);
            }
        }

        rb.velocity = transform.right * speed * Time.deltaTime;

        // if (transform.rotation.eulerAngles.z < 190f || transform.rotation.eulerAngles.z > 350f) {
        if (transform.rotation.eulerAngles.z < 215f || transform.rotation.eulerAngles.z > 325f) {
            float testAngle = transform.rotation.eulerAngles.z;
            if (testAngle > 180f) testAngle -= 180f;
            float difference = Mathf.Abs(90f - testAngle);

            strain += (Time.deltaTime * 0.02f * (90f - difference));
            RefreshStemStrengthVisual();
        }

        if (strain >= 4f) {
            EndRoot(false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Collision")) {
            EndRoot(false);
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Water")) {
            if (nutrientCount > 0)
                EndRoot(true);
            else
                EndRoot(false);
        }
    }

    void EndRoot(bool successfulPlant) {
        // Snapping the root to where the collider landed looks more natural
        stem.SetPosition(stem.positionCount - 1, circleCollider.transform.position);

        // Before destroying this root's controller, we create a copy static root
        GameObject plantedRoot = Instantiate(RootMinigameManager.Instance.plantedRootPrefab, transform.position, Quaternion.identity);
        PlantedRoot plantedRootDetail = plantedRoot.GetComponent<PlantedRoot>();
        plantedRootDetail.stem.positionCount = stem.positionCount;
        Vector3[] positionCopy = new Vector3[stem.positionCount];
        stem.GetPositions(positionCopy);
        plantedRootDetail.stem.SetPositions(positionCopy);
        plantedRootDetail.stem.startColor = stem.startColor;
        plantedRootDetail.stem.endColor = stem.endColor;
        plantedRootDetail.stem.endWidth = stem.endWidth;
        plantedRootDetail.hasFlower = successfulPlant;

        if (!successfulPlant) {
            plantedRootDetail.stem.startColor = Color.red;
            plantedRootDetail.stem.endColor = Color.red;
        }

        RootMinigameManager.Instance.SaveRoots();

        Destroy(this.gameObject);
    }

    public void AddNutrient() {
        nutrientCount++;
        currentPointer = tieredPointers[nutrientCount];
        foreach (SpriteRenderer pointer in tieredPointers) {
            if (currentPointer != pointer) pointer.enabled = false;
            else pointer.enabled = true;
        }
    }

    public void RefreshStemStrengthVisual() {
        float newScaleFactor = 1f - (strain / 4f);
        transform.localScale = new Vector3(newScaleFactor, newScaleFactor, newScaleFactor);
        UpdateColor(new Color(strain / 3f, 1f, 0f));
        stem.endWidth = 0.25f - (strain / 22f);
    }

    void UpdateColor(Color c) {
        currentPointer.color = c;
        stem.endColor = c;
    }
}
