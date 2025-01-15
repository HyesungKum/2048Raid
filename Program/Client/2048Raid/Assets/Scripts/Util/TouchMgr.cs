using UnityEngine;

public delegate void DirEvent(DIR dir);

public class TouchMgr : MonoBehaviourSingleton<TouchMgr>
{
    [Header("Touch OverView")]
    [SerializeField] private int touchCount = 0;
    [SerializeField] private int sensitive = 7000;
    [SerializeField] private Touch touchInfo;

    private Vector2 touchVector;
    private DIR dragDir;

    private bool touchUpConfirmed = false;
    private bool dragConfirmed = false;

    public DirEvent DraggingEvent;
    public DirEvent DragUpEvent;

    private void Update()
    {
        touchCount = Input.touchCount;

        if (touchCount != 0)
        {
            touchInfo = Input.GetTouch(0);

            switch (touchInfo.phase)
            {
                case TouchPhase.Began: saveTouchData(); break;
                case TouchPhase.Stationary: calculateDir(); break; 
                case TouchPhase.Ended: outDragResult(); break;
            }
        }
    }

    private void saveTouchData()
    {
        dragConfirmed = false;
        touchUpConfirmed = false;
        touchVector = touchInfo.position;
    }
    private void calculateDir()
    {
        float xvlaue = touchInfo.position.x - touchVector.x;
        float yvlaue = touchInfo.position.y - touchVector.y;

        float powerdX = xvlaue * xvlaue;
        float powerdY = yvlaue * yvlaue;

        //��ġ��ġ�� ũ�� ������ ������ ���� ����
        if (powerdX < sensitive && powerdY < sensitive)
        {
            dragConfirmed = false;
            return;
        }

        if (powerdX < powerdY) //���Ʒ�
        {
            if (yvlaue < 0) dragDir = DIR.DOWN;
            else dragDir = DIR.UP;
        }
        else //�¿�
        {
            if (xvlaue < 0) dragDir = DIR.LEFT;
            else dragDir = DIR.RIGHT;
        }
        
        //�巡�� ���϶� �߻��ϴ� �̺�Ʈ
        DraggingEvent?.Invoke(dragDir);

        dragConfirmed = true;
    }
    private void outDragResult()
    {
        if (!dragConfirmed) return;

        dragConfirmed = false;
        touchUpConfirmed = true;

        DragUpEvent?.Invoke(dragDir);
    }


    //===============================================================================
    //
    // �巡�� ��� ��� �޼���
    //
    public bool GetDragUp(out DIR dir)
    {
        dir = dragDir;

        if (!touchUpConfirmed) return false;

        touchUpConfirmed = false;

        return true;
    }
}
