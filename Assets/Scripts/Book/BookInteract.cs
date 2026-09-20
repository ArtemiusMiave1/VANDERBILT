using UnityEngine;

public class BookInteraction : MonoBehaviour
{
    public BookBase book;
      public void OpenBook()
    {
        if (book != null)
        {
            book.OpenBook();
        }

    }

}