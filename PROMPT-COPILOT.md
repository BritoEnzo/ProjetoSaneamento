# Prompt para o GitHub Copilot (modo Agent), caso prefira gerar o app por ele

Cole o texto abaixo no Copilot Chat do Visual Studio, em modo **Agent**, com o projeto
.NET MAUI `SaneaBrasil` recém-criado e aberto:

---

Você vai implementar, neste projeto .NET MAUI (.NET 10, C#) chamado SaneaBrasil, um app que
mostra o índice de saneamento básico de municípios brasileiros. Siga TODOS os requisitos:

**Arquitetura e POO**
- Padrão MVVM, com pastas Models, Services, ViewModels, Views e Converters.
- Herança: classe abstrata `Localidade` (Id, Nome, propriedade abstrata Descricao) com subclasses `Estado` (Sigla, Regiao) e `Municipio` (Uf, NomeCompleto).
- Polimorfismo: classe abstrata `IndicadorSaneamento` (Nome, Icone, TabelaSidra, PalavraChaveClassificacao, TermosAdequados, MetaMarcoLegal virtual, TotalDomicilios e DomiciliosAdequados com `private set`, Percentual, Nivel, método virtual `CategoriaEhAdequada(string)` e método `RegistrarCategoria(string, long)`), com subclasses `AbastecimentoAgua` (tabela 6804, adequado = "rede geral", meta 99%), `EsgotamentoSanitario` (tabela 6805, adequado = "rede geral" ou "fossa séptica", nunca "rudimentar", meta 90%) e `ColetaLixo` (tabela 6892, adequado = "coletado", "caçamba" ou "serviço de limpeza").
- Classe `IndiceSaneamento` que compõe o Municipio, os 3 indicadores e a população, com IndiceGeral = média dos indicadores com dados. Enum `NivelSaneamento` (Crítico <50, Baixo <70, Regular <85, Bom <95, Excelente).
- Classe abstrata `ApiServiceBase` (HttpClient, UrlBase abstrata, `GetAsync<T>` genérico com System.Text.Json, PropertyNameCaseInsensitive e AllowReadingFromString), tratando erros com uma `ApiException` de mensagem amigável.
- Interfaces `ILocalidadesService`, `ISaneamentoService`, `ICepService`, `IFavoritosService` e `INavegacaoService`, todas registradas por injeção de dependência no MauiProgram (páginas e ViewModels também).
- `BaseViewModel` implementando INotifyPropertyChanged, com Carregando, MensagemErro, SetProperty e um ExecutarAsync que trata erros.

**3 APIs (todas gratuitas e sem chave)**
1. IBGE Localidades: `https://servicodados.ibge.gov.br/api/v1/localidades/estados?orderBy=nome` e `.../estados/{UF}/municipios?orderBy=nome`.
2. IBGE Agregados v3 (SIDRA), Censo 2022: para cada tabela (6804, 6805, 6892), leia `https://servicodados.ibge.gov.br/api/v3/agregados/{tabela}/metadados`. Escolha a variável cuja unidade não tenha "%" e a classificação cujo nome contenha a palavra-chave. Use só as categorias de primeiro nível (nivel > 0, sem "Total"), para não contar duas vezes. Depois busque `.../{tabela}/periodos/-1/variaveis/{var}?localidades=N6[{codigoIbge}]&classificacao={idClassif}[{ids}]`. Valores "-" ou "..." valem 0. População: tabela 4714, variável 93.
3. ViaCEP: `https://viacep.com.br/ws/{cep}/json/`. Use o campo "ibge" como código do município e trate a resposta com "erro".

**Telas (7) e os 3 tipos de navegação, usando Shell**
- Flyout: AppShell com FlyoutItem "Saneamento" (que contém as abas) e FlyoutItem "Sobre o app".
- Tabs dentro do primeiro FlyoutItem: Início (busca por CEP + botão "Explorar por estado", que vai para `//estados`), Estados (CollectionView das UFs) e Favoritos (lista salva em Preferences, em JSON).
- Pilha/Push com Routing.RegisterRoute: Estados → "municipios" (SearchBar que filtra sem acento) → "detalhe". Passe os objetos via `GoToAsync(rota, IDictionary<string, object>)` e receba-os com IQueryAttributable.
- Detalhe: nome, população, cartão com o índice geral colorido pelo nível (IValueConverter NivelParaCorConverter), um cartão por indicador (%, ProgressBar, "X de Y domicílios", meta do Marco Legal), botão de favoritar e RefreshView.
- Modal: rota "sobredados", página com `Shell.PresentationMode="ModalAnimated"`, explicando o cálculo e com botão Fechar (`GoToAsync("..")`).
- Use bindings compilados (x:DataType) em todas as páginas e DataTemplates, e textos em português do Brasil.

Ao final, apague MainPage.xaml, garanta que o projeto compila sem erros para Windows e Android, e liste os arquivos criados.

---
