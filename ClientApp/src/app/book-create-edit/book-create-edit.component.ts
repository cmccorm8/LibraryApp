import { Component, Input, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { BooksService } from '../services/books.service';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-book-create-edit',
  templateUrl: './book-create-edit.component.html',
  styleUrls: ['./book-create-edit.component.css']
})
export class BookCreateEditComponent implements OnInit {

  bookList$!: Observable<any[]>;

  constructor(private service:BooksService) { }

  @Input() book:any;
  id: number = 0;
  title: string = "";
  authorName: string = "";
  isbnNumber: string = "";

  ngOnInit(): void {
    this.id = this.book.id;
    this.title = this.book.title;
    this.authorName = this.book.authorName;
    this.isbnNumber = this.book.isbnNumber;
    //this.bookList$ = this.service.getAllBooks();
  }

  createBook() {
    var book = {
      title:this.title,
      authorName:this.authorName,
      isbnNumber:this.isbnNumber
    }
    this.service.createBook(book);  
  }


  updateBook(): void {
    var book = {
      id:this.id,
      title:this.title,
      authorName:this.authorName,
      isbnNumber:this.isbnNumber
    }
    this.service.updateBook(book)
  }

}
