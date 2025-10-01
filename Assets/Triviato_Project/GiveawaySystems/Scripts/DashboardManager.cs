using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DashboardManager : AdvancedMono
{
    public List<Giveaway> Gifts = new List<Giveaway>();

    public GameObject Content;
    public GameObject DashBoardEntityPrefab;

    public List<GiveawayDashboardEntity> Entites;

    void Start()
    {
        foreach (var gift in Gifts)
        {
            gift.Load();
            var n = Instantiate(DashBoardEntityPrefab.gameObject, Content.transform);
            var n2 = n.GetComponent<GiveawayDashboardEntity>();
            n2.Load(gift);
            Entites.Add(n2);
        }
    }
    
    public void Save()
    {
        foreach (var entity in Entites)
        {
            entity.Save();
        }

        SceneManager.LoadScene("StartScreen 1");
    }


}
