using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillfeedUI : MonoBehaviour {

    public GameObject killFeedItemPrefab;
        
    public void OnKill(GameObject playerThatDied, GameObject playerThatKill)
    {
        GameObject item = (GameObject)Instantiate(killFeedItemPrefab, this.transform);
        item.GetComponent<KillfeedUIItem>().SetUp(playerThatDied, playerThatKill);
        item.transform.SetAsFirstSibling();

        Destroy(item, 4F);
    }
}
