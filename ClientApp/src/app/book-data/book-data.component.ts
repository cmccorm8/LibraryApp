import { Component, OnInit, OnDestroy } from '@angular/core';
import { Observable, Subscription } from 'rxjs';
import { BooksModel } from '../models/books.model';
import { BooksService } from '../services/books.service';
import { MatDialog } from '@angular/material/dialog';
import { MatDialogRef } from '@angular/material/dialog';



@Component({
  selector: 'app-book-data',
  templateUrl: './book-data.component.html',
  styleUrls: ['./book-data.component.css']
  
})
export class BookDataComponent implements OnInit, OnDestroy{
  public books: BooksModel[] = [];
  public displayedColumns = ['id', 'title', 'authorName', 'isbnNumber','edit', 'remove'];
  public subscriptions: Subscription[] = [];
  public bookList$!:Observable<any[]>;

  modalTitle: string ='';
  book: any;
  activateBookAddEditComponent: boolean = false;
  activateBookDeleteComponent: boolean = false;
  constructor(
    //private dialogRef: MatDialogRef<BookCreateComponent>,
    private bookService: BooksService,
    //private snackBar: MatSnackBar,
    private dialog: MatDialog
  ){}
   

  ngOnInit(): void {
    //this.getAllBooks();
 
    //bookList$ is a stream which makes a call to 
    //the API via the getAllBooks method in books.service.ts
    this.bookList$ = this.bookService.getAllBooks();
      
  }

  modalAdd() {
    this.book = {
      id:0,
      title:null,
      authorName:null,
      isbnNumber:null
    }
    this.modalTitle = "Add New Book";
    this.activateBookAddEditComponent = true;
  }

  modalDelete(item:any)
  {
    this.book=item;
    this.modalTitle = " Delete Book";
    this.activateBookDeleteComponent = true;
  }

  modalUpdate(item:any) {
    this.book=item;
    this.modalTitle = " Update Book";
    this.activateBookAddEditComponent = true;
  }

  modalClose() {
    this.activateBookAddEditComponent=false;
    this.bookList$ = this.bookService.getAllBooks();
  }

  deleteModalClose() {
    this.activateBookDeleteComponent=false;
    this.bookList$ = this.bookService.getAllBooks();
  }

  ngOnDestroy() {
    this.subscriptions.forEach(subscriptions => subscriptions.unsubscribe());
  }

  
  // getAllBooks() {
  //   const getAllBooksSubscription = this.bookService.getAllBooks()
  //     .subscribe(
  //       (resource: BooksModel[]) => {
  //         this.books = resource;
  //         console.log("response from api", resource);
  //       },
  //       (error: HttpErrorResponse) => {
  //         this.openSnackBar('Getting Books failed!, please try again later!', 'Dismiss');
  //         console.log(error);
  //       }
        
  //     );
  //     this.subscriptions.push(getAllBooksSubscription);
  // }

  

  /*private openSnackBar(message: string, action: string) {
    this.snackBar.open(message,action, {
      duration: 1000,
    });
  }*/

}


