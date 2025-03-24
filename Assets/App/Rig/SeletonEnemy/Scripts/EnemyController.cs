using UnityEngine;
using System.Threading.Tasks;
public class EnemyController : MonoBehaviour
{
    [SerializeField] private GameObject MainGameObject;
    [SerializeField] private GameObject Platfrom;

    public void DestroyPlatform()
    {
        DestroyPlatform(3);
    }

    async void DestroyPlatform(int seconds)
    {
        await Task.Delay(seconds * 1000);
        RemovePlatform();
    }

    void RemovePlatform()
    {
        Platfrom.SetActive(false);
        Debug.Log("Function called after 2 seconds!");

        Destroy(gameObject, 4f);
    }

   
}
