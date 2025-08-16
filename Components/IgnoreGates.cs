using TheHuntIsOn.Modules;
using UnityEngine;

namespace TheHuntIsOn.Components;

public class IgnoreGates : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collison)
    {
        if (!TheHuntIsOn.IsModuleUsed<IntangibleGatesModule>())
            return;
        if (collison.gameObject.tag == "Battle Gate" || collison.gameObject.name.Contains("Dream Gate"))
        {
            Physics2D.IgnoreCollision(GetComponent<BoxCollider2D>(), collison.gameObject.GetComponent<BoxCollider2D>());
        }
        else if(collison.transform.parent?.name == "Hornet Saver" || collison.gameObject.name == "Wall Saver" || collison.gameObject.name == "Enemy Saver" || collison.gameObject.name == "Floor Saver")
        {
            var colliders = collison.gameObject.GetComponents<BoxCollider2D>();
            foreach (BoxCollider2D collider2D in colliders)
                Physics2D.IgnoreCollision(GetComponent<BoxCollider2D>(), collider2D);
        }
        else if (collison.transform.parent?.name == "infected_door")
            Physics2D.IgnoreCollision(GetComponent<BoxCollider2D>(), collison.gameObject.GetComponent<PolygonCollider2D>());
        else if (collison.transform.name == "Colliders" && 
            ((UnityEngine.SceneManagement.SceneManager.GetActiveScene().name =="Deepnest_East_Hornet" && collison.transform.parent?.name == "Battle Scene")
            || (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Fungus2_15" && collison.transform.parent?.name == "mantis_cage_down")))
            foreach (BoxCollider2D collider in collison.gameObject.GetComponents<BoxCollider2D>())
                Physics2D.IgnoreCollision(GetComponent<BoxCollider2D>(), collider);
    }
}
