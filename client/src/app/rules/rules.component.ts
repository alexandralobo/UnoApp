import { Component, OnInit } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-rules',
  templateUrl: './rules.component.html',
  styleUrls: ['./rules.component.css']
})
export class RulesComponent implements OnInit {

  constructor(public dialogRef: MatDialogRef<RulesComponent>) { }

  ngOnInit(): void {
  }

  closeDialog() {
    this.dialogRef.close();
  }

}
