# SaneaBrasil — app .NET MAUI de saneamento básico por município

Consulta o índice de saneamento (água, esgoto e coleta de lixo) de bairros da Zona Leste de São Paulo,
com dados municipais do Censo 2022 do IBGE.

## Como montar o projeto no Visual Studio 2026

1. Abra o Visual Studio → **Criar um novo projeto** → **Aplicativo .NET MAUI** (.NET MAUI App).
2. Nome do projeto: **`SaneaBrasil`** (exatamente assim, porque os namespaces usam esse nome).
   Se aparecer a opção "Incluir conteúdo de exemplo", deixe **desmarcada**.
3. Com o projeto criado, **apague** `MainPage.xaml` (e o `MainPage.xaml.cs` junto).
4. Copie para a pasta do projeto (a que tem o `SaneaBrasil.csproj`) tudo o que está neste zip,
   **substituindo** `AppShell.xaml`, `AppShell.xaml.cs` e `MauiProgram.cs`:
   ```
   AppShell.xaml / AppShell.xaml.cs / MauiProgram.cs
   Converters/   Models/   Services/   ViewModels/   Views/
   ```
   Não mexa em `App.xaml`, `App.xaml.cs`, `Platforms/` nem `Resources/` (ficam como o modelo criou).
5. No topo do VS, escolha **Windows Machine** (ou um emulador Android) e aperte **F5**.

> As pastas novas aparecem sozinhas no Gerenciador de Soluções, porque projetos .NET incluem
> todos os arquivos da pasta. Se não aparecerem, clique em "Mostrar todos os arquivos".

## Requisitos do trabalho, e onde estão

| Requisito | Onde |
|---|---|
| **Orientação a objetos** | `Localidade` (abstrata) → `Estado`, `Municipio` (herança). `IndicadorSaneamento` (abstrata) → `AbastecimentoAgua`, `EsgotamentoSanitario`, `ColetaLixo` (polimorfismo: cada uma define sua tabela e o que é "adequado"; `EsgotamentoSanitario` sobrescreve `CategoriaEhAdequada`). `ApiServiceBase` (abstrata) → os 3 clientes de API. Encapsulamento (`private set`, métodos `RegistrarCategoria`). Interfaces + injeção de dependência (`Services/Interfaces.cs`, `MauiProgram.cs`). `BaseViewModel` para os ViewModels (MVVM). |
| **3+ APIs** | 1) **IBGE Localidades** (`IbgeLocalidadesService`); 2) **IBGE Agregados/SIDRA** (`IbgeSaneamentoService`: tabelas 6804, 6805, 6892 e 4714); 3) **ViaCEP** (`ViaCepService`). |
| **4+ telas** | 7 telas: Início, Estados, Municípios, Detalhe, Favoritos, Sobre o app, Como calculamos (modal). |
| **3 tipos de navegação** | **Flyout** (menu lateral ☰), **Tabs** (Início/Estados/Favoritos) e **Pilha/Push** (Estados → Municípios → Detalhe), além de uma tela **Modal**. Veja `AppShell.xaml`. |

## Como o índice é calculado

Para cada indicador, o app soma os domicílios do município em cada categoria da tabela do
Censo 2022 e calcula a porcentagem com atendimento adequado:

- **Água (6804):** rede geral de distribuição.
- **Esgoto (6805):** rede geral/pluvial (ou fossa ligada à rede) + fossa séptica/fossa filtro.
- **Lixo (6892):** coletado no domicílio ou em caçamba de serviço de limpeza.

O índice geral é a média simples dos três.
Faixas: <50 Crítico · 50–69 Baixo · 70–84 Regular · 85–94 Bom · ≥95 Excelente.

## Se der erro

- **"Não foi possível conectar…"**: confira a internet. No emulador Android, o modelo já traz a permissão de INTERNET.
- **Indicador "Sem dados"**: o IBGE pode estar fora do ar, ou o município pode não ter dados publicados para o Censo.
- **Dados por bairro**: a busca é limitada à Zona Leste de São Paulo, mas o IBGE disponibiliza os indicadores por município; por isso o app usa o índice municipal de São Paulo para todos os bairros selecionados.
- **Erro de compilação com `SaneaBrasil`**: o projeto foi criado com outro nome. Crie de novo com o nome `SaneaBrasil`.
