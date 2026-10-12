using UnityEngine;

public class ParticleSpawner : MonoBehaviour
{
    [SerializeField] GameObject particle;
    private ShopHandler shopHandler;
    private bool doParticle;

    public void Start()
    {
        if (shopHandler == null)
        {
            shopHandler = FindAnyObjectByType<ShopHandler>();
        }
    }

    public void SpawnParticle()
    {
        doParticle = false;
        switch(gameObject.name){
            case("Damage"):
                if(shopHandler.canBuyDamage){
                    doParticle = true;
                }
                break;
            case("Speed"):
                if(shopHandler.canBuySpeed){
                    doParticle = true;
                }
                break;
        }

        if(doParticle)
        {
            GameObject part = Instantiate(particle, transform);
            Destroy(part, 2f);
        }
    }
}