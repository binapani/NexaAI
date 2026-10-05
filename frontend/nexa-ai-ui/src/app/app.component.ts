
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ChatService, ChatResponse } from './services/chat.service';
import { CommonModule } from '@angular/common';
@Component({
  selector: 'app-root',
  standalone: true,
 imports: [CommonModule, FormsModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  title = 'NexaAI';

  message = '';
  conversationId: string | null = null;
  messages: { role: 'user' | 'assistant'; content: string }[] = [];
  loading = false;
  error = '';

  constructor(private chatService: ChatService) {}

  sendMessage(): void {
    const text = this.message.trim();

    if (!text || this.loading) {
      return;
    }

    this.messages.push({ role: 'user', content: text });
    this.message = '';
    this.loading = true;
    this.error = '';

    this.chatService.sendMessage({
      message: text,
      conversationId: this.conversationId
    }).subscribe({
      next: (response: ChatResponse) => {
        this.conversationId = response.conversationId;
        this.messages.push({
          role: 'assistant',
          content: response.reply
        });
        this.loading = false;
      },
      error: (err) => {
        console.error('NexaAI API error:', err);
        this.error = 'Unable to reach NexaAI API. Check that the .NET API is running.';
        this.loading = false;
      }
    });
  }
}
