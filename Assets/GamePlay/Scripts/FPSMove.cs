using UnityEngine;

namespace uGameCore.GamePlay
{
	public class FPSMove : MonoBehaviour
	{
		public	float	speed = 3.0f ;


		void Update() {

			if (this.IsLocalPlayer ()) {
				this.transform.position += this.transform.forward * Input.GetAxisRaw ("Vertical") * this.speed * Time.deltaTime;
				this.transform.position += this.transform.right * Input.GetAxisRaw ("Horizontal") * this.speed * Time.deltaTime;
			}

		}

	}
}

