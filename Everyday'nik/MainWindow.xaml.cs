using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Notify;
using System.Runtime.CompilerServices;

namespace Everyday_nik
{
    public class Note
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
    }

    public static class Serializer
    {
        public static void Serialize<T>(T data, string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                string json = JsonConvert.SerializeObject(data);
                writer.Write(json);
            }
        }

        public static T Deserialize<T>(string filePath)
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                string json = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<T>(json);
            }
        }
    }

    public class MainViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Note> _notes;
        private Note _selectedNote;
        private DateTime _selectedDate;

        public ObservableCollection<Note> Notes
        {
            get { return _notes; }
            set { _notes = value; OnPropertyChanged(); }
        }

        public Note SelectedNote
        {
            get { return _selectedNote; }
            set { _selectedNote = value; OnPropertyChanged(); }
        }

        public DateTime SelectedDate
        {
            get { return _selectedDate; }
            set { _selectedDate = value; LoadNotes(); }
        }

        public MainViewModel()
        {
            LoadNotes();
        }

        private void LoadNotes()
        {
            Notes = Serializer.Deserialize<ObservableCollection<Note>>("notes.json");
            Notes = Notes.Where(n => n.DueDate == SelectedDate).ToObservableCollection();
        }

        public void AddNote()
        {
            Note newNote = new Note() { DueDate = SelectedDate };
            Notes.Add(newNote);
            Serializer.Serialize(Notes, "notes.json");
        }

        public void EditNote()
        {
            if (SelectedNote != null)
            {
                Serializer.Serialize(Notes, "notes.json");
            }
        }

        public void DeleteNote()
        {
            if (SelectedNote != null)
            {
                Notes.Remove(SelectedNote);
                Serializer.Serialize(Notes, "notes.json");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}