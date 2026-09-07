using UnityEngine;

public class BookInteraction : MonoBehaviour
{
    public Book book;

    public void OpenBook()
    {
        if (book != null)
        {
            book.OpenBook();
        }
    }
}