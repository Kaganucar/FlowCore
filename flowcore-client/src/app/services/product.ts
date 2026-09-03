import { Service } from "@angular/core";
import { inject } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { ProductResponse } from "../Models/product.model";

@Service()
export class Product {
    private http = inject(HttpClient)
    private apiUrl = 'http://localhost:8080/api/product';

    getProducts(){
        return this.http.get<ProductResponse[]>(this.apiUrl);
    }
}
