import { Component, Input, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { BooksService } from '../services/books.service';

@Component({
  selector: 'app-book-delete',
  templateUrl: './book-delete.component.html',
  styleUrls: ['./book-delete.component.css']
})
export class BookDeleteComponent implements OnInit {

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
  }

  deleteBook(id:any)
  {
    console.log('Delete ID:', this.id);
    
    this.service.deleteBook(id);
  }

}
