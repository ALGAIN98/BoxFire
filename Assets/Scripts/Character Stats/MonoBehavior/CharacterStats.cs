using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//玩家属性战斗数据
public class CharacterStats : MonoBehaviour
{
    public CharacterData_SO characterData;

    public AttackData_SO attackData;

    [HideInInspector]
    public bool isCritical;

    #region Read from Data_SO
    public int CurrentHealth
    {
        get { if (characterData != null) return characterData.currentHealth; else return 0; }
        set { characterData.currentHealth = value; }
    }

    public int MaxHealth
    {
        get { if (characterData != null) return characterData.maxHealth; else return 0; }
        set { characterData.maxHealth = value; }
    }

    public int BaseDefence
    {
        get { if (characterData != null) return characterData.baseDefence; else return 0; }
        set { characterData.baseDefence = value; }
    }

    public int CurrentDefence
    {
        get { if (characterData != null) return characterData.currentDefence; else return 0; }
        set { characterData.currentDefence = value; }
    }

    #endregion



    #region Character Combat

    //伤害数值=攻击者的攻击力 - 防御者的防御力

    public void TakeDamage(CharacterStats attacker, CharacterStats defencer)
    {
        //受到的伤害最少为0，不可能为负数
        int damage = Mathf.Max(attacker.CurrentDamage() - defencer.CurrentDefence, 0);

        //血量最少为0，不可能为负数
        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);


        if (isCritical)
        {
            defencer.GetComponent<Animator>().SetTrigger("Hit");
        }

        //TODO: Update UI.
        //TODO：经验升级update
    }

    private int CurrentDamage()
    {
        float coreDamage = UnityEngine.Random.Range(attackData.minDamage, attackData.maxDamage);

        if (isCritical)
        {
            coreDamage *= attackData.criticalMutipliers;
            Debug.Log("暴击！" + coreDamage);
        }

        return (int)coreDamage;
    }


    #endregion
}
