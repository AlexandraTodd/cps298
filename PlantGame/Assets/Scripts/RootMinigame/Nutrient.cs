using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nutrient : MonoBehaviour {
    public SpriteRenderer auraSprite;
    [HideInInspector] public float auraSpriteAnimationDuration = 0f;
    [HideInInspector] public int index = -1;

    void Update() {
        auraSpriteAnimationDuration += 2f * Time.deltaTime;
        float newAuraSize = Mathf.Sin(auraSpriteAnimationDuration) * (RootMinigameManager.Instance.activeRoot == null && RootMinigameManager.Instance.delayBeforeReturnToDefaultView == 0f ? 9f : 3f);
        auraSprite.transform.localScale = new Vector3(newAuraSize, newAuraSize, newAuraSize);
    }

    private void FixedUpdate() {
        if (RootMinigameManager.Instance.activeRoot != null) {
            if (Vector3.Distance(transform.position, RootMinigameManager.Instance.activeRoot.transform.position) <= 1f) {
                // Root updates own nutrient count and graphic
                RootMinigameManager.Instance.activeRoot.AddNutrient();

                // Report to save file that this nutrient will no longer be available
                RootMinigameManager.Instance.nutrientsEnabled[index] = false;

                // Destroy nutrient after grabbed
                Destroy(gameObject);
            }
        }
    }
}
