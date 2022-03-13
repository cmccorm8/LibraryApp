import { HttpClient, HttpErrorResponse, HttpResponse } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { Observable, of } from "rxjs";
import { BooksModel } from "../models/books.model";
import { MatSnackBar } from '@angular/material/snack-bar';


@Injectable({
    providedIn: 'root'
})
export class BooksService {
    private readonly apiUrl: string = "https://localhost:5001/";
    
    
    constructor(
        private http: HttpClient,
        public snackBar: MatSnackBar,
        @Inject('BASE_URL') baseUrl: string
        ){}

    getAllBooks(): Observable<BooksModel[]> {
        const http$ = this.http.get<BooksModel[]>(`${this.apiUrl}api/books`);
        
        http$.subscribe(
            res => console.log('HTTP response', res),
            (error: HttpErrorResponse) => {
                this.openSnackBar('Getting Books failed! Please try again later!', 'Dismiss');
                console.log(error);
            }
        )

        return http$;
        //return this.http.get<BooksModel[]>(`${this.apiUrl}api/books`);
    }

    createBook(data:any): Observable<BooksModel> {
        const http$ = this.http.post<BooksModel>(`${this.apiUrl}api/books`, data);

        http$.subscribe(
            () => {
                //closes the modal after creating a new book
                var closeModalBtn = document.getElementById('add-edit-modal-close');
                if(closeModalBtn) {
                    closeModalBtn.click();
                }
                this.openSnackBar('Book Created Successfully!', 'Dismiss');
                console.log('Status: 201');
                console.log('StatusText: Created');
            },
            (err: HttpErrorResponse) => {
                switch(err.status) {
                    case 400:
                        this.openSnackBar('Book failed validation! Please review your data input for errors.', 'Dismiss');
                        break;
                    default:
                        this.openSnackBar('Something went wrong! Please try again later.', 'Dismiss');
                        break;
                }
                console.log(err);
            }
            
        );
        return http$;
    }

    updateBook(data:any): Observable<BooksModel> {
        const http$ = this.http.put(`${this.apiUrl}api/books`, data);

        http$.subscribe(
            () => {
                //closes the modal after updating a book
                var closeModalBtn = document.getElementById('add-edit-modal-close');
                if(closeModalBtn) {
                    closeModalBtn.click();
                }
                this.openSnackBar('Book Updated Successfully!', 'Dismiss');
                console.log('Status: 200');
                console.log('StatusText: OK');
            },
            (err: HttpErrorResponse) => {
                switch(err.status) {
                    case 400:
                        this.openSnackBar('Book failed validation! Please review your data input for errors.', 'Dismiss');
                        break;
                    default:
                        this.openSnackBar('Something went wrong! Please try again later.', 'Dismiss');
                        break;
                }
                console.log(err);
            }
            
        );
        return http$;
    }

    deleteBook(id: number): Observable<void> {
        const http$ = this.http.delete(`${this.apiUrl}api/books/${id}`);

        http$.subscribe(
            () => {
                var closeModalBtn = document.getElementById('delete-modal-close');
                if(closeModalBtn) {
                    closeModalBtn.click();
                }
                this.openSnackBar('Book Deleted Successfully!', 'Dismiss');
                console.log('Status: 200');
                console.log('StatusText: OK');
            },
            (err: HttpErrorResponse) => {
                this.openSnackBar('Delete Failed! Please try again later!', 'Dismiss');
                console.log(err);
            });
            
            return of();
            //return this.http.delete<void>(`${this.apiUrl}api/books/${id}`);
    }


    private openSnackBar(message: string, action: string) {
        this.snackBar.open(message,action, {
          duration: 10000,
          panelClass: ['warning']
        });
    }
}
