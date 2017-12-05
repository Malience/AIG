using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyArrayTable {
    private ByteStack enemylist;
    private ByteStack grouplist;


    public EnemyArrayTable(int enemies, int groups) {
        this.enemylist = new ByteStack(enemies);
        this.grouplist = new ByteStack(groups);
        for (byte i = 0; i < enemies; i++) enemylist.push(i);
        for (byte i = 0; i < groups; i++) grouplist.push(i);
    }
    public int size() { return enemylist.size(); }
    public bool hasEnemies(int len) { return enemylist.size() >= len; }
    public byte[] createEnemy(int len)
    {
        byte[] ids = new byte[len];
        for (int i = 0; i < len; i++) ids[i] = enemylist.pop();
        return ids;
    }

    public void destroyEnemy(byte enemy) { enemylist.push(enemy); }

    public bool hasGroup() { return !grouplist.isEmpty(); }
    public byte createGroup() { return grouplist.pop(); }
    public void destroyGroup(byte group) { grouplist.push(group); }

}
