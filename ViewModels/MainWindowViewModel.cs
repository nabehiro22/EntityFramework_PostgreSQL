using Microsoft.EntityFrameworkCore;
using Prism.Mvvm;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reactive.Disposables;

namespace EntityFramework_PostgreSQL.ViewModels
{
	public class MainWindowViewModel : BindableBase, INotifyPropertyChanged
	{
		/// <summary>
		/// データベースの構成
		/// </summary>
		[Table("memberlist")]
		public class MenberList
		{
			/// <summary>
			/// プログラムでは入力しない連番
			/// </summary>
			[Key]
			[Column("id")]
			public long Id { get; set; }

			/// <summary>
			/// 名前
			/// </summary>
			[Required]
			[Column("name")]
			public string Name { get; set; }

			/// <summary>
			/// 年齢
			/// </summary>
			[Required]
			[Column("age")]
			public int Age { get; set; }
		}

		/// <summary>
		/// データベース用のDBコンテキスト
		/// </summary>
		public class Db : DbContext
		{
			public DbSet<MenberList> Menber { get; set; }
			protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
			{
				optionsBuilder.UseNpgsql("Host=localhost; Port=5432; Username=postgres; Password=pass; Database=postgres; SearchPath=public");
			}
		}

		/// <summary>
		/// タイトル
		/// </summary>
		public ReactivePropertySlim<string> Title { get; } = new ReactivePropertySlim<string>("メンバーリスト");

		/// <summary>
		/// Disposeが必要な処理をまとめてやる
		/// </summary>
		private CompositeDisposable Disposable { get; } = new CompositeDisposable();

		/// <summary>
		/// MainWindowのCloseイベント
		/// </summary>
		public ReactiveCommand ClosedCommand { get; } = new ReactiveCommand();

		/// <summary>
		/// メンバー追加
		/// </summary>
		public ReactiveCommand AddCommand { get; } = new ReactiveCommand();

		/// <summary>
		/// メンバー削除
		/// </summary>
		public ReactiveCommand DeleteCommand { get; } = new ReactiveCommand();

		/// <summary>
		/// 追加する名前
		/// </summary>
		public ReactivePropertySlim<string> AddName { get; } = new ReactivePropertySlim<string>();

		/// <summary>
		/// 追加する年齢
		/// </summary>
		public ReactivePropertySlim<string> AddAge { get; } = new ReactivePropertySlim<string>();

		/// <summary>
		/// 削除する名前
		/// </summary>
		public ReactivePropertySlim<string> DeleteName { get; } = new ReactivePropertySlim<string>();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public MainWindowViewModel()
		{
			AddCommand.Subscribe(addData).AddTo(Disposable);
			DeleteCommand.Subscribe(deleteData).AddTo(Disposable);
			ClosedCommand.Subscribe(Close).AddTo(Disposable);
		}

		/// <summary>
		/// ウィンドウが閉じられるイベント
		/// </summary>
		private void Close()
		{
			Disposable.Dispose();
		}

		/// <summary>
		/// データベースへ追加
		/// </summary>
		private void addData()
		{
			using var db = new Db();
			var newMenber = new MenberList { Name = AddName.Value, Age = int.Parse(AddAge.Value) };
			db.Menber.Add(newMenber);
			db.SaveChanges();
		}

		/// <summary>
		///  データベースから削除
		/// </summary>
		private void deleteData()
		{
			using var db = new Db();
			var deleteMember = db.Menber.First(m => m.Name == DeleteName.Value);
			db.Menber.Remove(deleteMember);
			db.SaveChanges();
		}
	}
}
