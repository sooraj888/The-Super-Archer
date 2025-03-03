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

        await Task.Delay((seconds + 2) * 1000);
        Destroy(gameObject,2f);
    }

    void RemovePlatform()
    {
        Platfrom.SetActive(false);
        Debug.Log("Function called after 2 seconds!");
    }

   
}
