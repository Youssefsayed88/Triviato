using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LineCreator : MonoBehaviour {

	int vertexCount = 0;
	bool mouseDown = false;
	LineRenderer line;

	public GameObject blast;

	public GameObject splash;

	public int maxLines;
	void Awake () {
		line = GetComponent<LineRenderer> ();
	}

	void Start () {
		
	}

	void Update () {
		
		if (Input.GetMouseButtonDown (0)) {
			mouseDown = true;
		}

		if (mouseDown) {
			line.positionCount = vertexCount + 1;
			if (line.positionCount >= maxLines)
			{
				mouseDown = false;
				line.positionCount = 0;
				vertexCount = 0;

				BoxCollider2D[] colliders = GetComponents<BoxCollider2D> ();
				foreach (BoxCollider2D box2 in colliders) {
					Destroy (box2);
				}
				return;
			}
			Vector3 mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			line.SetPosition (vertexCount, mousePos);
			vertexCount++;

			BoxCollider2D box = gameObject.AddComponent<BoxCollider2D> ();
			box.transform.position = line.transform.position;
			box.size = new Vector2 (0.1f, 0.1f);
		}

		if (Input.GetMouseButtonUp (0)) {
			mouseDown = false;
			line.positionCount = 0;
			vertexCount = 0;

			BoxCollider2D[] colliders = GetComponents<BoxCollider2D> ();
			foreach (BoxCollider2D box in colliders) {
				Destroy (box);
			}
		}
		
	} // Update

	public int BombCount = 0;
	public bool isBombLocked = false;
	public IEnumerator AddBombCount()
	{
		yield return new WaitForSeconds (0.2f);
		isBombLocked = false;
	}

	public UnityEvent OnBomb;
	void OnCollisionEnter2D (Collision2D target) {
		if (target.gameObject.tag == "Bomb") {
			GameObject b = Instantiate (blast, target.transform.position, Quaternion.identity) as GameObject;
			Destroy (b.gameObject, 2f);
			Destroy (target.gameObject);
			if (isBombLocked == false)
			{
				BombCount++;
				StartCoroutine(AddBombCount());
				isBombLocked = true;
			}
			if (BombCount >= Config.Instance.settings["bombCountToDie"].GetValueOrDefault(2))
			{
				OnBomb.Invoke();
			}
		}

/*		if (target.gameObject.tag == "Fruit") {
			GameObject s = Instantiate (splash, new Vector3(target.transform.position.x -1, target.transform.position.y,0), Quaternion.identity) as GameObject;
			Destroy (s.gameObject, 2f);
			Destroy (target.gameObject);

		}*/
	}

} // LineCreator