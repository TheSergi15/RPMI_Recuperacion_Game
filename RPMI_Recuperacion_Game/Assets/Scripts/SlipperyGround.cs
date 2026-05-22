using UnityEngine;

public class SlipperyGround : MonoBehaviour
{
    
    void Start()
    {
        if (!CompareTag("SlipperyGround"))
            gameObject.tag = "SlipperyGround";

        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        if(sprite != null)
        {
            sprite.color = new Color(0.7f,0.9f, 1f, 1f);    
        }
        Debug.Log("Suelo resbaladizo creado: " +  gameObject.name);
    }

    
}
