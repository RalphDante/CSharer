# CSharer

Automated video distribution system for social media platforms.

## Features

- AI-generated viral captions (Groq)
- Auto-post to Buffer (YouTube, TikTok, Instagram)
- Auto-post to Pinterest
- Folder watching for new videos
- Automated workflow

## Prerequisites

- .NET 8.0 SDK
- API keys for: OpenAI, Buffer, Pinterest

## Setup

1. Clone the repo
```bash
git clone https://github.com/YOUR_USERNAME/CSharer.git
cd CSharer
```

2. Install dependencies
```bash
dotnet restore
```

3. Configure API keys
```bash
cp CSharer.Console/appsettings.example.json CSharer.Console/appsettings.json
cp client_secret.example.json client_secret.json
# Edit appsettings.json with your API keys
```

4. Run
```bash
dotnet run --project CSharer.Console
```

## Project Structure

- `CSharer.Core` - Shared services and models
- `CSharer.Console` - Console application
- `CSharer.WinForms` - Windows Forms UI
- `CSharer.Database` - MySQL Services

## Development

We use branch workflow:
1. Create feature branch from `develop`
2. Make changes
3. Submit PR to `develop`
4. Get review & merge

## Team

- [COMANDANTE] - Lead Developer
- [GALUA] - Database & CRUD
- [BABAYSON] - WinForms/ UI
- [REYES] - Documentation & Testing